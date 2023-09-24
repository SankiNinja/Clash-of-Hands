using System.Collections.Generic;
using ClashOfHands.Systems;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClashOfHands.UI
{
    public struct SettingsValues
    {
        public int Music;
        public int SFX;
    }

    public interface ISettingChangeReceiver
    {
        void OnSettingsChanged(SettingsValues values);
    }

    public interface ISettingsChangeProvider
    {
        void RegisterForSettingChanges(ISettingChangeReceiver receiver);
        void UnRegisterFromSettingChanges(ISettingChangeReceiver receiver);
    }

    public class SettingsMenu : MonoBehaviour, ISettingsChangeProvider
    {
        [FormerlySerializedAs("_musicToggle")]
        [SerializeField]
        private SettingsSlider _musicSlider;

        [FormerlySerializedAs("_sfxToggle")]
        [SerializeField]
        private SettingsSlider _sfxSlider;

        private readonly List<ISettingChangeReceiver> _changeReceivers = new(4);

        private SettingsValues _values;

        public void Initialize()
        {
            LoadData();
        }

        public void OnMusicValueChanged(float value)
        {
            _values.Music = (int)value;
            BroadcastSettingsChanges();
            SaveData();
        }

        public void OnSFXValueChanged(float value)
        {
            _values.SFX = (int)value;
            BroadcastSettingsChanges();
            SaveData();
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("Music", _values.Music);
            PlayerPrefs.SetInt("SFX", _values.SFX);
        }

        private void LoadData()
        {
            _values.Music = PlayerPrefs.GetInt("Music", 4);
            _values.SFX = PlayerPrefs.GetInt("SFX", 10);

            BroadcastSettingsChanges();

            _musicSlider.SetValue(_values.Music);
            _sfxSlider.SetValue(_values.SFX);
        }

        private void BroadcastSettingsChanges()
        {
            foreach (var receiver in _changeReceivers)
                receiver.OnSettingsChanged(_values);
        }

        public void RegisterForSettingChanges(ISettingChangeReceiver receiver)
        {
            receiver.OnSettingsChanged(_values);
            _changeReceivers.Add(receiver);
        }

        public void UnRegisterFromSettingChanges(ISettingChangeReceiver receiver)
        {
            _changeReceivers.UnRegisterReceiver(receiver);
        }
    }
}