using ClashOfHands.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class CardShowUI : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;

        public void SetUpCard(CardData cardData)
        {
            if (cardData == null)
                return;

            _icon.sprite = cardData.Sprite;
        }
    }
}