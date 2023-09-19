using ClashOfHands.Systems;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class PlayerShowCardPool : SerializedComponentPool<PlayerShowCardUI>
    {
        [SerializeField]
        [Range(0, 10)]
        private int _cardDistribution = 0;

        [SerializeField]
        [Range(0, 350)]
        private float spacing = 0;

        private void OnValidate()
        {
            UpdateCardPosition(_cardDistribution);
        }

        private void UpdateCardPosition(int cards)
        {
            if (cards == 0)
                return;

            var slice = 360 / cards;
            var childCount = transform.childCount;

            int rotation = 0;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(i < cards);
                if (i >= cards)
                    return;
                child.rotation = Quaternion.Euler(Vector3.forward * rotation);
                rotation += slice;
                var position = child.up * -spacing;
                child.transform.localPosition = position;
                child.SetAsFirstSibling();
            }
        }
    }
}