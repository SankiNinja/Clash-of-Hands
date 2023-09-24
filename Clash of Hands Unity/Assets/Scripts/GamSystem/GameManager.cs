using System.Collections.Generic;
using ClashOfHands.Data;
using UnityEngine;
using DG.Tweening;

namespace ClashOfHands.Systems
{
    public enum TurnState
    {
        TurnPrep,
        TurnStart,
        TurnEnd,
        GameOver,
    }

    public interface ITurnStateChangeReceiver
    {
        public void OnTurnUpdate(TurnState state);
    }

    public interface ITurnUpdateProvider
    {
        public void RegisterForTurnStateUpdates(ITurnStateChangeReceiver receiver);
        public void UnRegisterForTurnUpdates(ITurnStateChangeReceiver receiver);
        public void RegisterForTurnTickUpdates(ITimerTickReceiver receiver);
        public void UnRegisterForTurnUpdates(ITimerTickReceiver receiver);
    }

    public interface ICardInputReceiver
    {
        public int RegisterCardInputReceiver(ICardInputProvider cardInputProvider);
    }

    public interface IPollInputTrigger
    {
        void CollectInput();
    }

    public class GameManager : MonoBehaviourSingleton<GameManager>, ICardInputReceiver, ITurnUpdateProvider,
        ITimerTickReceiver, IPollInputTrigger
    {
        [SerializeField]
        private GameData _gameData;

        [SerializeField]
        private AvatarDatabase _avatarDatabase;

        [SerializeField]
        private float _turnTime = 1f;

        [SerializeField]
        private Timer _timer;

        [SerializeField]
        private AIPlayers _aiPlayers;

        [SerializeField]
        private Player _player;

        [SerializeField]
        [Range(1, 3)]
        private int _hearts;

        private CardData[] _inputCards;
        private int[] _totalScores;
        private int[] _roundResults;
        private int[] _normalizedScores;

        private Sprite[] _avatarSprites;

        private int _playerIndices;

        private Sprite[] AvatarSprites => _avatarDatabase.Avatars;

        private ICardInputProvider[] _cardInputs;
        private readonly List<ITurnStateChangeReceiver> _turnStateChangeReceivers = new(8);
        private readonly List<ITimerTickReceiver> _turnTickReceivers = new(4);

        private void Start()
        {
            _prepTurnCallback = PerpTurn;
            _startTurnCallback = StartTurn;

            var playerCount = _gameData.Players;
            _cardInputs = new ICardInputProvider[playerCount];
            _avatarSprites = new Sprite[playerCount];
            _inputCards = new CardData[playerCount];
            _totalScores = new int[playerCount];
            _roundResults = new int[playerCount];
            _normalizedScores = new int[playerCount];

            _player.LoadPlayerData();
            //_player.SetAvatar(_player.AvatarId, AvatarSprites[_player.AvatarId]);

            MainMenuManager.Instance.Initialize(_gameData.GameCards);
            ShowMenu();
        }

        public void ShowMenu()
        {
            MainMenuManager.Instance.ShowState(MainMenuManager.States.MainMenu);
            MainMenuManager.Instance.SetHighScore(_player.HighScore);
            SoundManager.Instance.SetMenuMusic();
            GameHUD.Instance.Hide();
        }

        private void ClearCachedValues()
        {
            for (int i = 0; i < _inputCards.Length; i++)
                _inputCards[i] = null;

            for (int i = 0; i < _totalScores.Length; i++)
                _totalScores[i] = 0;

            for (int i = 0; i < _roundResults.Length; i++)
                _roundResults[i] = 0;

            for (int i = 0; i < _normalizedScores.Length; i++)
                _normalizedScores[i] = 0;

            for (int i = 0; i < _avatarSprites.Length; i++)
                _avatarSprites[i] = null;

            for (int i = 0; i < _cardInputs.Length; i++)
                _cardInputs[i] = null;

            _turnTickReceivers.Clear();
            _turnStateChangeReceivers.Clear();
        }

        public void InitGame()
        {
            ClearCachedValues();

            RegisterInputSources();
            InitializeGameUI();
            ShowCountdown();

            SoundManager.Instance.SetGameMusic();
        }

        private void RegisterInputSources()
        {
            _playerIndices = 0;

            //Set Player
            _player.SetPlayerIndex(_playerIndices);
            _player.Hearts = _hearts;
            GameHUD.Instance.RegisterPlayerInput(this);

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
            GameHUD.Instance.Initialize(_gameData, _hearts, turnUpdateProvider: this, pollInputTrigger: this);
        }

        private void ShowCountdown()
        {
            GameHUD.Instance.ShowCountdown(_prepTurnCallback);
        }

        private TweenCallback _prepTurnCallback;

        private void PerpTurn()
        {
            BroadcastTurnUpdate(TurnState.TurnPrep);

            if (_player.Hearts == 0)
            {
                BroadcastTurnUpdate(TurnState.GameOver);

                int highScore = _player.HighScore;
                int currentScore = _totalScores[_player.PlayerIndex];

                GameHUD.Instance.ShowGamOver(currentScore, highScore);

                if (currentScore <= highScore)
                    return;

                _player.HighScore = currentScore;
                _player.SaveData();

                return;
            }

            var setupTime = GameHUD.Instance.GetDeckSetupDuration();
            DOVirtual.DelayedCall(setupTime, _startTurnCallback);
        }

        private TweenCallback _startTurnCallback;

        private void StartTurn()
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

        private void EndTurn()
        {
            BroadcastTurnUpdate(TurnState.TurnEnd);
            PollCardInput(_inputCards);

            _gameData.Evaluate(_inputCards, _roundResults);

            for (int i = 0; i < _roundResults.Length; i++)
            {
                var score = _totalScores[i];
                if (_roundResults[i] == 1)
                    _totalScores[i] = score + _roundResults[i];
            }

            var playerIndex = _player.PlayerIndex;
            var playerRoundResult = _roundResults[playerIndex];
            _player.Hearts += playerRoundResult;
            _player.Hearts = Mathf.Clamp(_player.Hearts, 0, _hearts);

            GameHUD.Instance.UpdateHUDPostTurn(_inputCards, _totalScores[playerIndex],
                playerRoundResult, playerIndex, _player.Hearts,
                _prepTurnCallback);
        }

        private void BroadcastTurnUpdate(TurnState state)
        {
            foreach (var turnBeginHandle in _turnStateChangeReceivers)
                turnBeginHandle?.OnTurnUpdate(state);
        }

        public void RegisterForTurnStateUpdates(ITurnStateChangeReceiver receiver)
        {
            _turnStateChangeReceivers.Add(receiver);
        }

        public void UnRegisterForTurnUpdates(ITurnStateChangeReceiver receiver)
        {
            _turnStateChangeReceivers.UnRegisterReceiver(receiver);
        }

        private void BroadcastTurnTicks(float currentTime, float targetTime)
        {
            foreach (var timerTickHandler in _turnTickReceivers)
            {
                timerTickHandler.OnTimerTicked(currentTime, targetTime);
            }
        }

        public void RegisterForTurnTickUpdates(ITimerTickReceiver receiver)
        {
            _turnTickReceivers.Add(receiver);
        }

        public void UnRegisterForTurnUpdates(ITimerTickReceiver receiver)
        {
            _turnTickReceivers.UnRegisterReceiver(receiver);
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

        public void CollectInput()
        {
            _timer.StopTimer();
            EndTurn();
        }
    }
}