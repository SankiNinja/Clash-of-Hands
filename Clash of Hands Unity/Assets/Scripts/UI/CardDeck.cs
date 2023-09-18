using System.Collections.Generic;
using ClashOfHands.UI;
using UnityEngine;

namespace ClashOfHands.Data
{
    public class CardDeck : MonoBehaviour, ICardClickHandler
    {
        [SerializeField]
        private CardPool _cardPool;

        private List<CardUI> _cardUIs = new();

        public void SetUpDeck(CardData[] cards)
        {
            Clear();

            foreach (var cardData in cards)
            {
                var card = _cardPool.Get();
                _cardUIs.Add(card);

                card.SetCardData(cardData, clickHandler: this);
                card.transform.SetAsLastSibling();
                card.gameObject.SetActive(true);
            }
        }

        public void OnCardClicked(CardData cardData, CardUI uiView)
        {
        }

        private void Clear()
        {
            foreach (var card in _cardUIs)
                card.Free();

            _cardUIs.Clear();
        }
    }

    public interface ICardClickHandler
    {
        void OnCardClicked(CardData cardData, CardUI uiView);
    }
}