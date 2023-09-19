using ClashOfHands.Data;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public class GameHUD : MonoBehaviourSingleton<GameHUD>
    {
        [SerializeField]
        private GameObject _visual;

        [SerializeField]
        private CardDeck _carDeck;

        public void SetUpGameFromGameData(GameData gameData)
        {
            _visual.SetActive(true);
            _carDeck.SetUpDeck(gameData.GameCards);
        }
    }
}