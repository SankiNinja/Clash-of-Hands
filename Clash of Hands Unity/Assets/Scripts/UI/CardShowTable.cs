using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class CardShowTable : MonoBehaviour
    {
        [SerializeField]
        private List<CardShowUI> _showCards;

        [SerializeField]
        [Range(0, 350)]
        private float spacing = 0;

        public void Initialize(int playerCount)
        {
            UpdateCardPosition(playerCount);
        }

        private void UpdateCardPosition(int players)
        {
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
            }
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