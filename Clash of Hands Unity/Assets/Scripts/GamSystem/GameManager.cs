using ClashOfHands.Data;
using NaughtyAttributes;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField]
        private GameData _gameData;

        [Button]
        public void SetUpGame()
        {
            GameHUD.Instance.SetUpGameFromGameData(_gameData);
        }

        private void Update()
        {
            
        }
    }
}