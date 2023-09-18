using System.Collections.Generic;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField]
        private List<CardUI> _pool;

        [SerializeField]
        private CardUI _cardPrefab;

        private void Start()
        {
            foreach (var cardUI in _pool)
            {
                cardUI.gameObject.SetActive(false);
                cardUI.Clear();
            }
        }

        public CardUI Get()
        {
            var lasIndex = _pool.Count - 1;
            var card = lasIndex < 0 ? Instantiate(_cardPrefab, transform) : _pool[^1];
            card.OwnerPool = this;
            _pool.RemoveAt(lasIndex);
            return card;
        }

        public void Release(CardUI card)
        {
            _pool.Add(card);

            card.gameObject.SetActive(false);
            card.Clear();
        }
    }
}