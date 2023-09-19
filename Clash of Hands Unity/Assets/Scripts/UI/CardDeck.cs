using System.Collections.Generic;
using ClashOfHands.Systems;
using ClashOfHands.UI;
using UnityEngine;

namespace ClashOfHands.Data
{
    public class CardDeck : MonoBehaviour, ICardClickHandler, ICardInputProvider, ITurnTimerExpiredHandler
    {
        [SerializeField]
        private CardPool _cardPool;

        [SerializeField]
        private TurnTimerUI _timerUI;

        private readonly List<CardUI> _cardUIs = new(8);

        private CardData _selectedCard;

        private ICardInputPoller _cardInputPoller;

        private float _turnTime;

        public void SetUpDeck(CardData[] cards, float turnTime)
        {
            Clear();

            _turnTime = turnTime;

            _cardPool.InitializePool();

            foreach (var cardData in cards)
            {
                var card = _cardPool.Get();
                _cardUIs.Add(card);

                card.SetCardData(cardData, clickHandler: this);
                card.transform.SetAsLastSibling();
                card.gameObject.SetActive(true);
            }
        }

        private void Clear()
        {
            foreach (var card in _cardUIs)
                ((IPoolObject<CardUI>)card).Free();

            _cardUIs.Clear();
        }

        public void StartRound()
        {
            _timerUI.StartTimer(_turnTime, this);
        }

        public void OnCardClicked(CardData cardData, CardUI uiView)
        {
            _selectedCard = cardData;
        }

        public void OnTimerExpired(float duration)
        {
            _cardInputPoller.PollCardInput();
        }

        public int PlayerIndex { get; set; }

        public void RegisterToInputPoller(ICardInputPoller inputPoller)
        {
            _cardInputPoller = inputPoller;
            PlayerIndex = inputPoller.RegisterCardInputReceiver(this);
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