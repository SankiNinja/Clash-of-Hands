using UnityEngine;

namespace ClashOfHands.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private ToggleVisual _musicToggle;

        [SerializeField]
        private ToggleVisual _sfxToggle;

        private bool _music;
        private bool _sfx;

        public void OnMusicToggle(bool isOn)
        {
            _music = isOn;
        }

        public void OnSFXToggle(bool isOn)
        {
            _sfx = isOn;
        }

        public void SaveData()
        {
            PlayerPrefs.SetInt("Music", _music ? 1 : 0);
            PlayerPrefs.SetInt("SFX", _sfx ? 1 : 0);
        }

        public void LoadData()
        {
            _music = PlayerPrefs.GetInt("Music", 1) == 1;
            _sfx = PlayerPrefs.GetInt("SFX", 1) == 1;

            _musicToggle.Toggle.isOn = _music;
            _sfxToggle.Toggle.isOn = _sfx;
        }
    }
}