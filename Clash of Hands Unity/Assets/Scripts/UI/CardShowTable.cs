using System.Collections.Generic;
using ClashOfHands.Data;
using ClashOfHands.Systems;
using DG.Tweening;
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
        private float spacing;

        private readonly List<CardShowUI> _activeCards = new(4);

        [SerializeField]
        private TweenTimeEase _moveIn;

        [SerializeField]
        private TweenTimeEase _scaleIn;

        [SerializeField]
        private TweenTimeEase _scaleOut = TweenTimeEase.OutExpo;

        public void Initialize(int playerCount, ITurnUpdateProvider turnUpdate)
        {
            gameObject.SetActive(true);

            UpdateCardPosition(playerCount);
            turnUpdate.RegisterForTurnStateUpdates(this);

            foreach (var activeCard in _activeCards)
                activeCard.gameObject.SetActive(false);
        }

        private void UpdateCardPosition(int players)
        {
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
            if (state != TurnState.TurnPrep)
                return;

            foreach (var cardShowUI in _activeCards)
                cardShowUI.transform.DOScale(Vector3.zero, _scaleOut.Time).SetEase(_scaleOut.Ease);
        }

        public void ShowCards(CardData[] cards, int playerCardIndex, RectTransform selectedCardRect,
            out float completionTime)
        {
            completionTime = _moveIn.Time;

            for (int i = 0; i < cards.Length; i++)
            {
                var showCardUI = _activeCards[i];
                if (cards[i] == null)
                    continue;

                showCardUI.gameObject.SetActive(true);
                
                var cardTransform = showCardUI.transform;
                cardTransform.localScale = Vector3.one;
                
                showCardUI.SetUpCard(cards[i]);

                if (i == playerCardIndex)
                {
                    if (selectedCardRect == null)
                    {
                        showCardUI.gameObject.SetActive(false);
                        continue;
                    }

                    var startPosition = cardTransform.position;
                    cardTransform.position = selectedCardRect.transform.position;
                    cardTransform.DOMove(startPosition, _moveIn.Time).SetEase(_moveIn.Ease);

                    var cardRect = cardTransform.GetComponent<RectTransform>();
                    var startSizeDelta = cardRect.sizeDelta;
                    cardRect.sizeDelta = selectedCardRect.sizeDelta;
                    cardRect.DOSizeDelta(startSizeDelta, _scaleIn.Time).SetEase(_scaleIn.Ease);
                    cardRect.DOSizeDelta(startSizeDelta, _scaleIn.Time).SetEase(_scaleIn.Ease);
                    continue;
                }

                showCardUI.GetComponent<BounceScale>().Bounce(null);
            }
        }

#if UNITY_EDITOR

        [Header("Editor Only")]
        [SerializeField]
        [Range(0, 10)]
        private int _cardDistribution;

        [Button]
        public void TestDistribution()
        {
            UpdateCardPosition(_cardDistribution);
        }
#endif
    }
}