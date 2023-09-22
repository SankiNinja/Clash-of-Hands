using System.Collections.Generic;
using ClashOfHands.Data;
using ClashOfHands.Systems;
using NaughtyAttributes;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class CardShowTable : MonoBehaviour, ITurnStateChangeReceiver
    {
        [SerializeField]
        private List<CardShowUI> _showCards;

        [SerializeField]
        [Range(0, 350)]
        private float spacing = 0;

        private CardData[] _gameCards;

        private readonly List<CardShowUI> _activeCards = new(4);

        public void Initialize(int playerCount, CardData[] gameCards, ITurnUpdateProvider turnUpdate)
        {
            _gameCards = gameCards;
            UpdateCardPosition(playerCount);
            turnUpdate.RegisterForTurnStateUpdates(this);

            foreach (var activeCard in _activeCards)
                activeCard.gameObject.SetActive(false);
        }

        private void UpdateCardPosition(int players)
        {
            foreach (var activeCard in _activeCards)
                activeCard.StopAnimation();

            _activeCards.Clear();

            if (players == 0)
                return;

            var slice = 360 / players;

            int rotation = 0;
            for (int i = 0; i < _showCards.Count; i++)
            {
                var card = _showCards[i].transform;
                card.gameObject.SetActive(i < players);
                if (i >= players)
                    continue;

                card.rotation = Quaternion.Euler(Vector3.forward * rotation);
                rotation += slice;
                var position = card.up * -spacing;
                card.transform.localPosition = position;
                card.SetAsFirstSibling();
                _activeCards.Add(_showCards[i]);
            }
        }

        public void OnTurnUpdate(TurnState state)
        {
            switch (state)
            {
                case TurnState.TurnStart:
                    foreach (var cardShowUI in _activeCards)
                        cardShowUI.AnimateCardIcon(_gameCards);
                    break;
                case TurnState.TurnEnd:
                    foreach (var cardShowUI in _activeCards)
                        cardShowUI.StopAnimation();
                    break;
                case TurnState.Wait:
                    break;
            }
        }

        public void ShowCards(CardData[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
                _activeCards[i].SetUpCard(cards[i]);
        }

#if UNITY_EDITOR
        [SerializeField]
        [Range(0, 10)]
        private int _cardDistribution = 0;

        [Button]
        private void TestDistribution()
        {
            UpdateCardPosition(_cardDistribution);
        }
#endif
    }
}