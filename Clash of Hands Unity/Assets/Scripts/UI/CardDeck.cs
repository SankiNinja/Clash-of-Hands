using System.Collections.Generic;
using ClashOfHands.Data;
using ClashOfHands.Systems;
using DG.Tweening;
using UnityEngine;

namespace ClashOfHands.UI
{
    public interface ICardClickHandler
    {
        void OnCardClicked(CardData cardData, CardUI uiView);
    }

    public class CardDeck : MonoBehaviour, ICardClickHandler, ICardInputProvider, ITurnStateChangeReceiver
    {
        [SerializeField]
        private CardPool _cardPool;

        [SerializeField]
        private TurnTimerUI _timerUI;

        [SerializeField]
        private TweenTimeEase _scaleIn = TweenTimeEase.InExpo;

        [SerializeField]
        private TweenTimeEase _scaleOut = TweenTimeEase.OutExpo;

        private readonly List<CardUI> _cardUIs = new(8);

        private CardData _selectedCard;
        private RectTransform _selectedCardRect;

        private IPollInputTrigger _pollInputTrigger;

        public RectTransform SelectedCardRect => _selectedCardRect;

        private TurnState _turnState;

        public float PerpTime => Mathf.Max(_scaleIn.Time, _scaleOut.Time);

        public void SetUpDeck(CardData[] cards, ITurnUpdateProvider turnUpdateProvider,
            IPollInputTrigger pollInputTrigger)
        {
            turnUpdateProvider.RegisterForTurnStateUpdates(this);
            _pollInputTrigger = pollInputTrigger;

            Clear();
            _cardPool.InitializePool();
            foreach (var cardData in cards)
            {
                var card = _cardPool.Get();
                _cardUIs.Add(card);

                card.SetCardData(cardData, clickHandler: this);
                card.transform.SetAsLastSibling();
                card.gameObject.SetActive(false);
            }

            _timerUI.Initialize(turnUpdateProvider);
            _timerUI.gameObject.SetActive(false);
        }

        private void Clear()
        {
            foreach (var card in _cardUIs)
                ((IPoolObject<CardUI>)card).Free();

            _cardUIs.Clear();
        }

        public void OnCardClicked(CardData cardData, CardUI uiView)
        {
            if (_turnState != TurnState.TurnStart)
                return;

            _selectedCard = cardData;
            _selectedCardRect = uiView.GetComponent<RectTransform>();
            _pollInputTrigger?.CollectInput();
            AnimateOutDeck();
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

        public void OnTurnUpdate(TurnState state)
        {
            if (state == TurnState.TurnPrep)
                AnimateInDeck();

            _turnState = state;
        }

        private void AnimateInDeck()
        {
            foreach (var card in _cardUIs)
            {
                card.gameObject.SetActive(true);
                card.transform.localScale = Vector3.zero;
                card.transform.DOScale(Vector3.one, _scaleIn.Time).SetEase(_scaleIn.Ease);
            }

            _timerUI.gameObject.SetActive(true);
            _timerUI.OnTimerTicked(1, 1);
            _timerUI.GetComponent<CanvasGroup>().DOFade(1, _scaleIn.Time).SetEase(_scaleIn.Ease);
        }

        private void AnimateOutDeck()
        {
            foreach (var card in _cardUIs)
            {
                card.transform.localScale = Vector3.one;
                card.transform.DOScale(Vector3.zero, _scaleOut.Time).SetEase(_scaleOut.Ease);
            }

            _timerUI.GetComponent<CanvasGroup>().DOFade(0, _scaleIn.Time).SetEase(_scaleIn.Ease);
        }
    }
}