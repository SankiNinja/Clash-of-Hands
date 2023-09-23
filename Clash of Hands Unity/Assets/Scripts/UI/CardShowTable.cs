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
        private float spacing = 0;

        private CardData[] _gameCards;

        private readonly List<CardShowUI> _activeCards = new(4);

        [SerializeField]
        private TweenTimeEase _moveIn;

        [SerializeField]
        private TweenTimeEase _scaleIn;

        [SerializeField]
        private TweenTimeEase _scaleOut = TweenTimeEase.OutExpo;

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
            if (state != TurnState.TurnPrep)
                return;

            foreach (var cardShowUI in _activeCards)
                cardShowUI.transform.DOScale(Vector3.zero, _scaleOut.Time).SetEase(_scaleOut.Ease);
        }

        public void ShowCards(CardData[] cards, int playerCardIndex, RectTransform cardRect, out float completionTime)
        {
            completionTime = _moveIn.Time;

            for (int i = 0; i < cards.Length; i++)
            {
                _activeCards[i].gameObject.SetActive(true);
                _activeCards[i].transform.localScale = Vector3.one;
                _activeCards[i].SetUpCard(cards[i]);

                if (i == playerCardIndex)
                {
                    var activeCardTransform = _activeCards[i].transform;
                    var startPosition = activeCardTransform.position;
                    activeCardTransform.position = cardRect.transform.position;
                    activeCardTransform.DOMove(startPosition, _moveIn.Time).SetEase(_moveIn.Ease);

                    var activeCardRect = activeCardTransform.GetComponent<RectTransform>();
                    var startSizeDelta = activeCardRect.sizeDelta;
                    activeCardRect.sizeDelta = cardRect.sizeDelta;
                    activeCardRect.DOSizeDelta(startSizeDelta, _scaleIn.Time).SetEase(_scaleIn.Ease);
                    activeCardRect.DOSizeDelta(startSizeDelta, _scaleIn.Time).SetEase(_scaleIn.Ease);
                    continue;
                }

                _activeCards[i].GetComponent<BounceScale>().Bounce(null);
            }
        }

#if UNITY_EDITOR

        [Header("Editor Only")]
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