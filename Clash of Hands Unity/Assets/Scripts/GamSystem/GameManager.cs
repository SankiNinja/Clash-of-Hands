using System.Collections.Generic;
using ClashOfHands.Data;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

namespace ClashOfHands.Systems
{
    public enum TurnState
    {
        TurnStart,
        TurnEnd,
        Wait,
    }

    public enum GameMode
    {
        Arcade,
        Elimination,
    }

    public interface ITurnStateChangeReceiver
    {
        public void OnTurnUpdate(TurnState state);
    }

    public interface ITurnUpdateProvider
    {
        public void RegisterForTurnStateUpdates(ITurnStateChangeReceiver receiver);
        public void UnRegisterForTurnUpdates(ITurnStateChangeReceiver receiver);
        public void RegisterForTurnTickUpdates(ITimerTickHandler receiver);
        public void UnRegisterForTurnUpdates(ITimerTickHandler receiver);
    }

    public interface ICardInputReceiver
    {
        public int RegisterCardInputReceiver(ICardInputProvider cardInputProvider);
    }

    public class GameManager : MonoBehaviourSingleton<GameManager>, ICardInputReceiver, ITurnUpdateProvider,
        ITimerTickHandler
    {
        private enum GameState
        {
            GameStart,
            Countdown,
            PreTurn,
            InTurn,
            PostTurn,
            ScoreEvaluation,
            GameResolution,
            Pause,
            Wait,
        }

        [SerializeField]
        private GameData _gameData;

        [SerializeField]
        private AvatarDatabase _avatarDatabase;

        [SerializeField]
        private float _turnTime = 1f;

        [SerializeField]
        private Timer _timer;

        [SerializeField]
        private GameMode _gameMode;

        [SerializeField]
        private AIPlayers _aiPlayers;

        [SerializeField]
        private Player _player;

        private ICardInputProvider[] _cardInputs;
        private int[] _playerScores;
        private int[] _playerRoundResults;
        private CardData[] _playedCards;
        private int[] _normalizedScores;

        private Sprite[] _avatarSprites;

        private int _playerIndices = 0;

        private Sprite[] AvatarSprites => _avatarDatabase.Avatars;

        private readonly List<ITurnStateChangeReceiver> _turnStateChangeReceivers = new(8);
        private readonly List<ITimerTickHandler> _turnTickHandlers = new(4);

        private TweenCallback _startTurnCallback;

        private GameState _state;

        private void Start()
        {
            _startTurnCallback = StartTurn;

            var playerCount = _gameData.Players;
            _cardInputs = new ICardInputProvider[playerCount];
            _avatarSprites = new Sprite[playerCount];
            _playedCards = new CardData[playerCount];
            _playerScores = new int[playerCount];
            _playerRoundResults = new int[playerCount];
            _normalizedScores = new int[playerCount];

            var hasPlayerData = _player.LoadPlayerData();
            if (hasPlayerData == false)
                MainMenuManager.Instance.ShowState(MainMenuManager.States.AvatarSelection);
            else
            {
                _player.SetAvatar(_player.AvatarId, AvatarSprites[_player.AvatarId]);
                MainMenuManager.Instance.ShowState(MainMenuManager.States.MainMenu);
            }

            GameHUD.Instance.Hide();

            _state = GameState.Wait;
        }

        public void ResetGame()
        {
            for (int i = 0; i < _playedCards.Length; i++)
                _playedCards[i] = null;

            for (int i = 0; i < _playerScores.Length; i++)
                _playerScores[i] = 0;

            for (int i = 0; i < _playerRoundResults.Length; i++)
                _playerRoundResults[i] = 0;

            for (int i = 0; i < _normalizedScores.Length; i++)
                _normalizedScores[i] = 0;

            for (int i = 0; i < _avatarSprites.Length; i++)
                _avatarSprites[i] = null;
        }

        private void Update()
        {
            switch (_state)
            {
                case GameState.GameStart:
                    SetState(GameState.Wait);
                    break;

                case GameState.Countdown:
                    SetState(GameState.Wait);

                    ShowCountdown();
                    break;

                case GameState.PreTurn:
                    SetState(GameState.InTurn);
                    break;

                case GameState.InTurn:
                    SetState(GameState.Wait);
                    break;

                case GameState.PostTurn:
                    SetState(GameState.ScoreEvaluation);
                    break;

                case GameState.ScoreEvaluation:
                    SetState(GameState.ScoreEvaluation);
                    break;

                case GameState.GameResolution:
                    SetState(GameState.Wait);
                    break;

                case GameState.Pause:
                    //Do nothing I guess.
                    break;

                case GameState.Wait:

                    break;
            }
        }

        private void ShowCountdown()
        {
            GameHUD.Instance.ShowCountdown(_startTurnCallback);
        }

        private void SetState(GameState state)
        {
            _state = state;
        }

        public void SetUpGame()
        {
            RegisterInputSources();
            InitializeGameUI();
            SetState(GameState.Countdown);
        }

        private void RegisterInputSources()
        {
            //Set Player
            _player.SetPlayerIndex(_playerIndices);
            GameHUD.Instance.RegisterPlayerInput(this, _player.PlayerIndex);

            //Set AI
            _aiPlayers.Initialize(_gameData, this, _gameData.Players - 1);

            for (int i = 0; i < _cardInputs.Length; i++)
            {
                if (i == _player.PlayerIndex)
                {
                    _avatarSprites[i] = AvatarSprites[_player.AvatarId];
                    continue;
                }

                _avatarSprites[i] = _avatarDatabase.GetRandomAvatar(out _);
            }
        }

        private void InitializeGameUI()
        {
            GameHUD.Instance.SetUpGameFromGameData(_gameData, turnUpdateProvider: this);
            GameHUD.Instance.SetScoreHUD(_avatarSprites, _player.PlayerIndex, _gameMode);
        }

        [Button]
        public void StartTurn()
        {
            BroadcastTurnUpdate(TurnState.TurnStart);
            _timer.StartTimer(_turnTime, this);
        }

        public void OnTimerTicked(float currentTime, float targetTime)
        {
            BroadcastTurnTicks(currentTime, targetTime);
            if (Mathf.Approximately(Mathf.InverseLerp(0, targetTime, currentTime), 1))
                EndTurn();
        }

        [Button]
        public void EndTurn()
        {
            BroadcastTurnUpdate(TurnState.TurnEnd);
            PollCardInput(_playedCards);

            _gameData.Evaluate(_playedCards, _playerRoundResults);

            for (int i = 0; i < _playerRoundResults.Length; i++)
            {
                var score = _playerScores[i];
                _playerScores[i] = score + _playerRoundResults[i];
            }

            UpdateHUD(_playedCards, _playerScores);

            if (_gameMode == GameMode.Elimination)
            {
                if (_playerRoundResults[_player.PlayerIndex] < 0)
                {
                    GameHUD.Instance.Hide();
                    MainMenuManager.Instance.ShowState(MainMenuManager.States.MainMenu);
                    return;
                }
            }

            DOVirtual.DelayedCall(2f, _startTurnCallback);
        }

        private void BroadcastTurnUpdate(TurnState state)
        {
            foreach (var turnBeginHandle in _turnStateChangeReceivers)
                turnBeginHandle?.OnTurnUpdate(state);
        }

        private void UpdateHUD(CardData[] cards, int[] playerScores)
        {
            //TODO : Add delay in evaluation and stuff.
            GameHUD.Instance.ShowCards(cards);
            GameHUD.Instance.UpdateScores(playerScores);
        }

        public void RegisterForTurnStateUpdates(ITurnStateChangeReceiver receiver)
        {
            _turnStateChangeReceivers.Add(receiver);
        }

        public void UnRegisterForTurnUpdates(ITurnStateChangeReceiver receiver)
        {
            var count = _turnStateChangeReceivers.Count;
            for (int i = 0; i < count; i++)
            {
                if (ReferenceEquals(_turnStateChangeReceivers[i], receiver))
                {
                    var lastHandler = _turnStateChangeReceivers[count - 1];
                    _turnStateChangeReceivers[i] = lastHandler;
                    _turnStateChangeReceivers.RemoveAt(count - 1);
                    count--;
                    i--;
                }
            }
        }

        private void BroadcastTurnTicks(float currentTime, float targetTime)
        {
            foreach (var timerTickHandler in _turnTickHandlers)
            {
                timerTickHandler.OnTimerTicked(currentTime, targetTime);
            }
        }

        public void RegisterForTurnTickUpdates(ITimerTickHandler receiver)
        {
            _turnTickHandlers.Add(receiver);
        }

        public void UnRegisterForTurnUpdates(ITimerTickHandler receiver)
        {
            var count = _turnTickHandlers.Count;
            for (int i = 0; i < count; i++)
            {
                if (ReferenceEquals(_turnTickHandlers[i], receiver))
                {
                    var lastHandler = _turnTickHandlers[count - 1];
                    _turnTickHandlers[i] = lastHandler;
                    _turnTickHandlers.RemoveAt(count - 1);
                    count--;
                    i--;
                }
            }
        }

        private void PollCardInput(CardData[] cards)
        {
            foreach (var cardInput in _cardInputs)
            {
                var playerIndex = cardInput.PlayerIndex;
                cards[playerIndex] = cardInput.GetCardInput();
            }
        }

        public int RegisterCardInputReceiver(ICardInputProvider cardInputProvider)
        {
            var currentIndex = _playerIndices;
            _playerIndices++;
            _cardInputs[currentIndex] = cardInputProvider;
            return currentIndex;
        }

        public void UpdatePlayerAvatar(int avatarId, Sprite sprite)
        {
            _player.SetAvatar(avatarId, sprite);
            _player.SaveData();
        }
    }
}