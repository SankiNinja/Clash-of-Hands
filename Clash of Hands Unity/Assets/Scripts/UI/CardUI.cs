using ClashOfHands.Data;
using ClashOfHands.Systems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClashOfHands.UI
{
    public class CardUI : MonoBehaviour , IPoolObject<CardUI>
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

        public void UpdateCardUI(string text, Sprite sprite)
        {
            _nameLabel.SetText(text);
            _image.sprite = sprite;
        }

        public void UpdateCardLabel(string text)
        {
            _nameLabel.SetText(text);
        }

        //Bound to Button Component on the Game Object.
        public void OnCardClicked()
        {
            _cardClickHandler.OnCardClicked(_currentCardData, this);
        }

        public SerializedComponentPool<CardUI> OwnerPool { get; set; }

        public void Clear()
        {
            _currentCardData = null;
            _cardClickHandler = null;
            _image.sprite = null;
            _nameLabel.SetText(string.Empty);
        }
    }
}