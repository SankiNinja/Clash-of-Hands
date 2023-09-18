using ClashOfHands.Data;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public class GameHUD : MonoBehaviourSingleton<GameHUD>
    {
        [SerializeField]
        private CardDeck _carDeck;

        public void SetUpGameFromGameData(GameData gameData)
        {
            _carDeck.SetUpDeck(gameData.GameCards);
        }
    }
}