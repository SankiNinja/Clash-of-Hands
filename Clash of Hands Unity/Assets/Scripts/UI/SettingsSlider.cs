using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class SettingsSlider : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textLabel;

        [SerializeField]
        private Slider _slider;

        public void SetValue(int value)
        {
            _slider.value = value;
            OnValueChanged(value);
        }

        public void OnValueChanged(float value)
        {
            _textLabel.SetText("{0}", (int)value);
        }
    }
}