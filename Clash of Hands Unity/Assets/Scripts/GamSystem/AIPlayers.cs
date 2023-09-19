using ClashOfHands.Data;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public interface ICardInputProvider
    {
        int PlayerIndex { get; set; }

        public void RegisterToInputPoller(ICardInputPoller inputPoller);
        CardData GetCardInput();
    }

    public class AIPlayers : MonoBehaviour
    {
        private GameData _gameData;

        private AICardInput[] _aiPlayers;

        public AICardInput this[int index] => _aiPlayers[index];
        public int Length => _aiPlayers.Length;

        public void Initialize(GameData gameData, ICardInputPoller inputPoller, int aiInstances, int avatarCount)
        {
            _gameData = gameData;

            //Create required AI instances.
            _aiPlayers = new AICardInput[aiInstances];
            for (var i = 0; i < aiInstances; i++)
            {
                var avatarIndex = Random.Range(0, avatarCount);
                var aiCardInput = new AICardInput(_gameData, avatarIndex);
                _aiPlayers[i] = aiCardInput;

                aiCardInput.RegisterToInputPoller(inputPoller);
            }
        }
    }

    public class AICardInput : ICardInputProvider
    {
        private GameData _gameData;
        public int PlayerIndex { get; set; }

        public int AvatarIndex { get; private set; }

        public AICardInput(GameData gameData, int avatarIndex)
        {
            _gameData = gameData;
            AvatarIndex = avatarIndex;
        }

        public void RegisterToInputPoller(ICardInputPoller inputPoller)
        {
            PlayerIndex = inputPoller.RegisterCardInputReceiver(this);
        }

        public CardData GetCardInput()
        {
            var cardCount = _gameData.GameCards.Length;
            var cardToPlay = Random.Range(0, cardCount);
            return _gameData.GameCards[cardToPlay];
        }
    }
}