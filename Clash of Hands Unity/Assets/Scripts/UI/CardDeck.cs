using System.Collections.Generic;
using ClashOfHands.Systems;
using ClashOfHands.UI;
using UnityEngine;

namespace ClashOfHands.Data
{
    public class CardDeck : MonoBehaviour, ICardClickHandler, ICardInputProvider
    {
        [SerializeField]
        private CardPool _cardPool;

        [SerializeField]
        private TurnTimerUI _timerUI;

        private readonly List<CardUI> _cardUIs = new(8);

        private CardData _selectedCard;

        public void SetUpDeck(CardData[] cards, ITurnUpdateProvider turnUpdateProvider)
        {
            Clear();
            _cardPool.InitializePool();
            foreach (var cardData in cards)
            {
                var card = _cardPool.Get();
                _cardUIs.Add(card);

                card.SetCardData(cardData, clickHandler: this);
                card.transform.SetAsLastSibling();
                card.gameObject.SetActive(true);
            }

            _timerUI.Initialize(turnUpdateProvider);
        }

        private void Clear()
        {
            foreach (var card in _cardUIs)
                ((IPoolObject<CardUI>)card).Free();

            _cardUIs.Clear();
        }

        public void OnCardClicked(CardData cardData, CardUI uiView)
        {
            _selectedCard = cardData;
        }

        public int PlayerIndex { get; set; }

        public void RegisterToInputPoller(ICardInputReceiver inputReceiver)
        {
            PlayerIndex = inputReceiver.RegisterCardInputReceiver(this);
        }

        public CardData GetCardInput()
        {
            return _selectedCard;
        }
    }

    public interface ICardClickHandler
    {
        void OnCardClicked(CardData cardData, CardUI uiView);
    }
}