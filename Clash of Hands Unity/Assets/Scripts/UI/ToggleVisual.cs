using ClashOfHands.Systems;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class ToggleVisual : MonoBehaviour
    {
        public Toggle Toggle;

        [Header("Fill Image")]
        [SerializeField]
        private Image _fillImage;

        [SerializeField]
        private Color _onFillColor;

        [SerializeField]
        private Color _offFillColor;

        [Header("Knob Color")]
        [SerializeField]
        private Image _knobImage;

        [SerializeField]
        private Color _onKnobColor;

        [SerializeField]
        private Color _offKobColor;

        [Header("Knob Position")]
        [SerializeField]
        private RectTransform _knobRect;

        [SerializeField]
        private float _onKnobPos;

        [SerializeField]
        private float _offKnobPos;

        [SerializeField]
        private GameUtils.TweenTimeEase _moveTime = GameUtils.TweenTimeEase.InExpo;

        public void SetState(bool isOn)
        {
            var fillColor = isOn ? _onFillColor : _offFillColor;
            var knobColor = isOn ? _onKnobColor : _offKobColor;
            var position = isOn ? _onKnobPos : _offKnobPos;

            _fillImage.DOKill();
            _fillImage.DOColor(fillColor, _moveTime.Time).SetEase(_moveTime.Ease);

            _knobImage.DOKill();
            _knobImage.DOColor(knobColor, _moveTime.Time).SetEase(_moveTime.Ease);

            _knobRect.DOKill();
            _knobRect.DOAnchorPosX(position, _moveTime.Time).SetEase(_moveTime.Ease);
        }
    }
}