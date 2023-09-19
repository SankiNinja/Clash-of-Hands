using UnityEngine;

namespace ClashOfHands.Systems
{
    public class MainMenuManager : MonoBehaviourSingleton<MainMenuManager>
    {
        [SerializeField]
        private GameObject _visual;
        
        public void OnPlayButtonClicked()
        {
            _visual.SetActive(false);
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
    }
}