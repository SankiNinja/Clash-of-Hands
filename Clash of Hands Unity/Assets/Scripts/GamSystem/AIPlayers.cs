using ClashOfHands.Data;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public interface ICardInputProvider
    {
        int PlayerIndex { get; set; }

        public void RegisterToInputPoller(ICardInputReceiver inputReceiver);
        CardData GetCardInput();
    }

    public class AIPlayers : MonoBehaviour
    {
        private GameData _gameData;

        private AICardInput[] _aiPlayers;

        public AICardInput this[int index] => _aiPlayers[index];
        public int Length => _aiPlayers.Length;

        public void Initialize(GameData gameData, ICardInputReceiver inputReceiver, int aiInstances)
        {
            _gameData = gameData;

            //Create required AI instances.
            _aiPlayers = new AICardInput[aiInstances];
            for (var i = 0; i < aiInstances; i++)
            {
                var aiCardInput = new AICardInput(_gameData);
                _aiPlayers[i] = aiCardInput;

                aiCardInput.RegisterToInputPoller(inputReceiver);
            }
        }
    }

    public class AICardInput : ICardInputProvider
    {
        private GameData _gameData;
        public int PlayerIndex { get; set; }

        public AICardInput(GameData gameData)
        {
            _gameData = gameData;
        }

        public void RegisterToInputPoller(ICardInputReceiver inputReceiver)
        {
            PlayerIndex = inputReceiver.RegisterCardInputReceiver(this);
        }

        public CardData GetCardInput()
        {
            var cardCount = _gameData.GameCards.Length;
            var cardToPlay = Random.Range(0, cardCount);
            return _gameData.GameCards[cardToPlay];
        }
    }
}