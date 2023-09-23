using System;
using ClashOfHands.Data;
using ClashOfHands.UI;
using TMPro;
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
        private GameTitle _title;

        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private GameObject _avatarSelection;

        [SerializeField]
        private SettingsMenu _settings;

        [SerializeField]
        private TextMeshProUGUI _highScoreText;

        public void OnPlayButtonClicked()
        {
            Hide();
            GameManager.Instance.InitGame();
        }

        public void OnQuitButtonClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }

        private void Hide()
        {
            _visual.SetActive(false);
            _title.Pause();
        }

        public void Initialize(CardData[] cards)
        {
            _title.Initialize(cards);
        }

        public void SetHighScore(int highScore)
        {
            _highScoreText.SetText("High Score : {0}", highScore);
            _highScoreText.gameObject.SetActive(highScore > 0);
        }

        public void ShowState(States state)
        {
            _visual.SetActive(true);
            _title.Play();
            _menu.SetActive(state == States.MainMenu);
            _settings.gameObject.SetActive(state == States.Settings);
            _avatarSelection.SetActive(state == States.AvatarSelection);
        }

        public void ShowMenu()
        {
            ShowState(States.MainMenu);
        }

        public void ShowSettings()
        {
            ShowState(States.Settings);
        }
    }
}