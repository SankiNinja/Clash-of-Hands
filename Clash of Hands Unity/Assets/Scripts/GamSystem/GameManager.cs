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

    public class GameManager : MonoBehaviourSingleton<GameManager>, ICardInputPoller
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

        public Sprite[] AvatarSprites => _avatarDatabase.Avatars;

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
                _player.SetAvatar(_player.PlayerIndex, AvatarSprites[_player.PlayerIndex]);
                MainMenuManager.Instance.ShowState(MainMenuManager.States.MainMenu);
                GameHUD.Instance.Hide();
            }
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
            InitializeGame();
        }

        private void RegisterInputSources()
        {
            //Set Player
            _player.SetPlayerIndex(_playerIndices);
            GameHUD.Instance.RegisterPlayerInput(this, _player.PlayerIndex);

            //Set AI
            _aiPlayers.Initialize(_gameData, this, _gameData.Players - 1, AvatarSprites.Length);
        }

        private void InitializeGame()
        {
            GameHUD.Instance.SetUpGameFromGameData(_gameData, _turnTime);
            GameHUD.Instance.SetScoreHUD(_player.AvatarSprite, _otherAvatarSprites);
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
    }
}