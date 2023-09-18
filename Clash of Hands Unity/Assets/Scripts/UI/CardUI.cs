using ClashOfHands.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClashOfHands.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _nameLabel;

        private CardData _currentCardData;

        private ICardClickHandler _cardClickHandler;

        public void SetCardData(CardData cardData, ICardClickHandler clickHandler)
        {
            _currentCardData = cardData;
            _cardClickHandler = clickHandler;

            UpdateCardUI(cardData.DisplayName, cardData.Sprite);
        }

        private void UpdateCardUI(string cardName, Sprite sprite)
        {
            _nameLabel.SetText(cardName);
            _image.sprite = sprite;
        }

        //Bound to Button Component on the Game Object.
        public void OnCardClicked()
        {
            _cardClickHandler.OnCardClicked(_currentCardData, this);
        }

        public CardPool OwnerPool { private get; set; }

        public void Free()
        {
            OwnerPool.Release(this);
        }

        public void Clear()
        {
            _currentCardData = null;
            _cardClickHandler = null;
            _image.sprite = null;
            _nameLabel.SetText(string.Empty);
        }
    }
}