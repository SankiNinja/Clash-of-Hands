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

        private TweenCallback _playAnnouncement;
        private TweenCallback _updateScores;
        private TweenCallback _startNewTurnCallback;

        private int[] _playerScores;
        private int[] _roundResults;

        public void Initialize(GameData gameData, ITurnUpdateProvider turnUpdateProvider,
            IPollInputTrigger pollInputTrigger)
        {
            _visual.SetActive(true);
            _cardDeck.SetUpDeck(gameData.GameCards, turnUpdateProvider, pollInputTrigger);
            _cardTable.Initialize(gameData.Players, gameData.GameCards, turnUpdateProvider);
            _countdown.Initialize(gameData.GameCards);
            _announcer.Initialize(gameData.AnnouncerData);

            _playAnnouncement = _announcer.PlayAnnouncement;
            _updateScores = UpdateScores;
        }

        public void RegisterPlayerInput(ICardInputReceiver inputReceiver)
        {
            _cardDeck.RegisterToInputPoller(inputReceiver);
        }

        public void SetScoreHUD(Sprite[] sprites, int playerIndex, GameMode gameMode)
        {
            _scoreboard.Initialize(sprites, playerIndex, gameMode);
        }

        public void Hide()
        {
            _visual.gameObject.SetActive(false);
        }

        public void UpdateHUDPostTurn(CardData[] cards, int[] playerScores, int[] roundResults, int playerIndex,
            TweenCallback startNewTurnCallback)
        {
            _startNewTurnCallback = startNewTurnCallback;
            _playerScores = playerScores;
            _roundResults = roundResults;

            _cardTable.ShowCards(cards, _cardDeck.PlayerIndex, _cardDeck.SelectedCardRect, out var showTime);

            _announcer.BuildAnnouncements(cards, roundResults[playerIndex], playerIndex, out var announcementTime);
            DOVirtual.DelayedCall(showTime, _playAnnouncement);

            DOVirtual.DelayedCall(showTime + announcementTime, _updateScores);
        }

        public float GetDeckSetupDuration()
        {
            return _cardDeck.PerpTime;
        }

        private void UpdateScores()
        {
            _scoreboard.UpdateScoreboard(_playerScores, _roundResults, out var duration);
            DOVirtual.DelayedCall(duration, _startNewTurnCallback);
        }

        public void ShowCountdown(TweenCallback startTurn)
        {
            _countdown.Animate(startTurn);
        }
    }
}