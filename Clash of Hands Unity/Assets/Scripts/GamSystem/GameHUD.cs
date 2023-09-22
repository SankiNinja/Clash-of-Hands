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
        private StartCountdown _countdown;

        public void SetUpGameFromGameData(GameData gameData, ITurnUpdateProvider turnUpdateProvider)
        {
            _visual.SetActive(true);
            _cardDeck.SetUpDeck(gameData.GameCards, turnUpdateProvider);
            _cardTable.Initialize(gameData.Players, gameData.GameCards, turnUpdateProvider);
            _countdown.Initialize(gameData.GameCards);
        }

        public void RegisterPlayerInput(ICardInputReceiver inputReceiver, int localPlayerIndex)
        {
            _cardDeck.RegisterToInputPoller(inputReceiver);
        }

        public void SetScoreHUD(Sprite[] sprites, int playerIndex, GameMode gameMode)
        {
            _scoreboard.Initialize(sprites, playerIndex, gameMode);
        }

        public void UpdateScores(int[] scores)
        {
            _scoreboard.UpdateScoreboard(scores);
        }

        public void Hide()
        {
            _visual.gameObject.SetActive(false);
        }

        public void ShowCards(CardData[] cards)
        {
            _cardTable.ShowCards(cards);
        }

        public void ShowCountdown(TweenCallback startTurn)
        {
            _countdown.Animate(startTurn);
        }
    }
}