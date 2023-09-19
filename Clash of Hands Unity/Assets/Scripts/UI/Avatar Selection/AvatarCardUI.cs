using ClashOfHands.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class AvatarCardUI : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Image _bg;

        [SerializeField]
        private Color _defaultBGColor;

        [SerializeField]
        private Color _selectedBGColor;

        [SerializeField]
        private int _index;


        private IAvatarCardClickHandler _cardClickHandler;

        public void Initialize(Sprite sprite, int index, IAvatarCardClickHandler cardClickHandler)
        {
            _image.sprite = sprite;
            _index = index;
            _cardClickHandler = cardClickHandler;
        }

        public void SetSelectedState(bool selected)
        {
            _bg.color = selected ? _selectedBGColor : _defaultBGColor;

            if (selected)
                _cardClickHandler?.OnSelected(_image.sprite, _index);
        }
    }
}