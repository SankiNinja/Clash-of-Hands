using ClashOfHands.Data;
using ClashOfHands.UI;
using UnityEngine;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("table")]
        [SerializeField]
        private CardShowTable _cardTable;

        private int _localPlayerIndex = 0;

        public void SetUpGameFromGameData(GameData gameData, float turnTime, ITurnUpdateProvider turnUpdate)
        {
            _visual.SetActive(true);
            _cardDeck.SetUpDeck(gameData.GameCards, turnTime);
            _cardTable.Initialize(gameData.Players, gameData.GameCards, turnUpdate);
        }

        public void RegisterPlayerInput(ICardInputPoller inputPoller, int localPlayerIndex)
        {
            _cardDeck.RegisterToInputPoller(inputPoller);
            _localPlayerIndex = localPlayerIndex;
        }

        public void SetScoreHUD(Sprite playerSprite, Sprite[] sprites)
        {
            _scoreboard.Initialize(sprites, playerSprite);
        }

        public void UpdateScores(int[] scores)
        {
            for (int i = 0; i < scores.Length; i++)
            {
                if (i != _localPlayerIndex)
                {
                    _scoreboard.UpdateScoreboard(i, scores[i]);
                    continue;
                }

                _scoreboard.UpdateLocalPlayerScore(scores[i]);
            }
        }

        public void Hide()
        {
            _visual.gameObject.SetActive(false);
        }
    }
}