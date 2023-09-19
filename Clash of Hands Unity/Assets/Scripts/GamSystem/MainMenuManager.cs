using System;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public class MainMenuManager : MonoBehaviourSingleton<MainMenuManager>
    {
        [Serializable]
        public enum States
        {
            MainMenu,
            Settings,
            AvatarSelection,
        }

        [SerializeField]
        private GameObject _visual;

        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private GameObject _avatarSelection;

        [SerializeField]
        private GameObject _settings;

        public void OnPlayButtonClicked()
        {
            Hide();
            GameManager.Instance.SetUpGame();
        }

        public void OnSettingsButtonClicked()
        {
        }

        public void OnQuitButtonClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }


        public void Hide()
        {
            _visual.SetActive(false);
        }

        public void ShowState(States state)
        {
            _visual.SetActive(true);
            _menu.SetActive(state == States.MainMenu);
            _settings.SetActive(state == States.Settings);
            _avatarSelection.SetActive(state == States.AvatarSelection);
        }
    }
}