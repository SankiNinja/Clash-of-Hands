using System.Collections.Generic;
using ClashOfHands.Data;
using NaughtyAttributes;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public interface ICardInputPoller
    {
        public int RegisterCardInputReceiver(ICardInputProvider cardInputProvider);
        public void PollCardInput();
    }

    public interface ITurnUpdateHandler
    {
        public void OnTurnBegin();
    }

    public interface ITurnUpdateProvider
    {
        public void RegisterForTurnUpdates(ITurnUpdateHandler handler);
        public void UnRegisterForTurnUpdates(ITurnUpdateHandler handler);
    }

    public class GameManager : MonoBehaviourSingleton<GameManager>, ICardInputPoller, ITurnUpdateProvider
    {
        [SerializeField]
        private GameData _gameData;

        [SerializeField]
        private AvatarDatabase _avatarDatabase;

        [SerializeField]
        private float _turnTime = 1f;

        [SerializeField]
        private AIPlayers _aiPlayers;

        [SerializeField]
        private Player _player;

        private ICardInputProvider[] _cardInputs;
        private int[] _playerScores;
        private int[] _playerResults;
        private CardData[] _playedCards;
        private int[] _normalizedScores;

        private Sprite[] _otherAvatarSprites;

        private int _playerIndices = 0;

        private Sprite[] AvatarSprites => _avatarDatabase.Avatars;

        private List<ITurnUpdateHandler> _turnBeginHandlers = new(8);

        private void Start()
        {
            var playerCount = _gameData.Players;
            _cardInputs = new ICardInputProvider[playerCount];
            _otherAvatarSprites = new Sprite[playerCount - 1];
            _playedCards = new CardData[playerCount];
            _playerScores = new int[playerCount];
            _playerResults = new int[playerCount];
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
        }

        public void ResetGame()
        {
            for (int i = 0; i < _playedCards.Length; i++)
                _playedCards[i] = null;

            for (int i = 0; i < _playerScores.Length; i++)
                _playerScores[i] = 0;

            for (int i = 0; i < _playerResults.Length; i++)
                _playerResults[i] = 0;

            for (int i = 0; i < _normalizedScores.Length; i++)
                _normalizedScores[i] = 0;

            for (int i = 0; i < _otherAvatarSprites.Length; i++)
                _otherAvatarSprites[i] = null;
        }

        [Button]
        public void SetUpGame()
        {
            RegisterInputSources();
            InitializeGameUI();
        }

        private void RegisterInputSources()
        {
            //Set Player
            _player.SetPlayerIndex(_playerIndices);
            GameHUD.Instance.RegisterPlayerInput(this, _player.PlayerIndex);

            //Set AI
            _aiPlayers.Initialize(_gameData, this, _gameData.Players - 1, AvatarSprites.Length);

            for (int i = 0; i < _aiPlayers.Length; i++)
                _otherAvatarSprites[i] = AvatarSprites[_aiPlayers[i].AvatarIndex];
        }

        private void InitializeGameUI()
        {
            GameHUD.Instance.SetUpGameFromGameData(_gameData, _turnTime, this);
            GameHUD.Instance.SetScoreHUD(_player.AvatarSprite, _otherAvatarSprites);
        }

        [Button]
        public void StartTurn()
        {
            foreach (var turnBeginHandle in _turnBeginHandlers)
                turnBeginHandle?.OnTurnBegin();
        }

        public void PollCardInput()
        {
            foreach (var cardInput in _cardInputs)
            {
                var playerIndex = cardInput.PlayerIndex;
                _playedCards[playerIndex] = cardInput.GetCardInput();
            }

            _gameData.Evaluate(_playedCards, _playerScores);
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

        public void RegisterForTurnUpdates(ITurnUpdateHandler handler)
        {
            _turnBeginHandlers.Add(handler);
        }

        public void UnRegisterForTurnUpdates(ITurnUpdateHandler handler)
        {
            var count = _turnBeginHandlers.Count;
            for (int i = 0; i < count; i++)
            {
                if (ReferenceEquals(_turnBeginHandlers[i], handler))
                {
                    var lastHandler = _turnBeginHandlers[count - 1];
                    _turnBeginHandlers[i] = lastHandler;
                    _turnBeginHandlers.RemoveAt(count - 1);
                    count--;
                    i--;
                }
            }
        }
    }
}