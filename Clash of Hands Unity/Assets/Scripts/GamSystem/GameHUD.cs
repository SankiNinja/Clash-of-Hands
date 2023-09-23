using ClashOfHands.Data;
using ClashOfHands.UI;
using DG.Tweening;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public class GameHUD : MonoBehaviourSingleton<GameHUD>
    {
        [SerializeField]
        private GameObject _visual;

        [SerializeField]
        private CardDeck _cardDeck;

        [SerializeField]
        private Scoreboard _scoreboard;

        [SerializeField]
        private CardShowTable _cardTable;

        [SerializeField]
        private Announcer _announcer;

        [SerializeField]
        private StartCountdown _countdown;

        [SerializeField]
        private GameOver _gameOver;

        private TweenCallback _playAnnouncement;
        private TweenCallback _updateScores;
        private TweenCallback _startNewTurnCallback;

        private int _playerScore;
        private int _roundResult;
        private int _playerHearts;

        public void Initialize(GameData gameData, int hearts, ITurnUpdateProvider turnUpdateProvider,
            IPollInputTrigger pollInputTrigger)
        {
            _playAnnouncement = _announcer.PlayAnnouncement;
            _updateScores = UpdateScores;

            _visual.SetActive(true);
            _gameOver.gameObject.SetActive(false);

            _cardDeck.Initialize(gameData.GameCards, turnUpdateProvider, pollInputTrigger);
            _cardTable.Initialize(gameData.Players, turnUpdateProvider);
            _countdown.Initialize();
            _announcer.Initialize(gameData.AnnouncerData);
            _scoreboard.Initialize(hearts);
        }

        public void RegisterPlayerInput(ICardInputReceiver inputReceiver)
        {
            _cardDeck.RegisterToInputPoller(inputReceiver);
        }

        public void Hide()
        {
            _visual.gameObject.SetActive(false);
            _gameOver.gameObject.SetActive(false);
        }

        public void UpdateHUDPostTurn(CardData[] cards, int playerScore, int roundResult, int playerIndex,
            int playerHearts,
            TweenCallback startNewTurnCallback)
        {
            _startNewTurnCallback = startNewTurnCallback;
            _playerScore = playerScore;
            _roundResult = roundResult;
            _playerHearts = playerHearts;

            _cardTable.ShowCards(cards, _cardDeck.PlayerIndex, _cardDeck.SelectedCardRect, out var showTime);

            _announcer.BuildAnnouncements(cards, _roundResult, playerIndex, out var announcementTime);

            DOVirtual.DelayedCall(showTime, _playAnnouncement);

            DOVirtual.DelayedCall(showTime + announcementTime, _updateScores);
        }

        public float GetDeckSetupDuration()
        {
            return _cardDeck.PerpTime;
        }

        private void UpdateScores()
        {
            _scoreboard.UpdateScoreboard(_playerScore, _roundResult, _playerHearts, out var duration);
            DOVirtual.DelayedCall(duration, _startNewTurnCallback);
        }

        public void ShowCountdown(TweenCallback startTurn)
        {
            _countdown.Animate(startTurn);
        }

        public void ShowGamOver(int currentScore, int highScore)
        {
            _visual.SetActive(false);
            _gameOver.SetGameOver(currentScore, highScore);
        }
    }
}