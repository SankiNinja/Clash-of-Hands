using ClashOfHands.Systems;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public interface IAvatarCardClickHandler
    {
        void OnSelected(Sprite sprite, int index);
    }

    public class AvatarSelectionUI : MonoBehaviour, IAvatarCardClickHandler
    {
        [SerializeField]
        private AvatarDatabase _avatarDatabase;

        [SerializeField]
        private AvatarCardUI _cardPefab;

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private Image _selectedAvatar;

        [SerializeField]
        private AvatarCardUI[] _avatarCards;

        private int _selectedAvatarIndex;

        public void Start()
        {
            var avatars = _avatarDatabase.Avatars;
            _selectedAvatarIndex = 0;
            _selectedAvatar.sprite = avatars[_selectedAvatarIndex];

            for (int i = 0; i < avatars.Length; i++)
            {
                _avatarCards[i].Initialize(avatars[i], i, this);
            }
        }

        public void OnSelected(Sprite sprite, int index)
        {
            _selectedAvatar.sprite = sprite;
            _selectedAvatarIndex = index;
        }

        public void OnSelectionConfirmed()
        {
            GameManager.Instance.UpdatePlayerAvatar(_selectedAvatarIndex, _selectedAvatar.sprite);
            MainMenuManager.Instance.ShowState(MainMenuManager.States.MainMenu);
        }

#if UNITY_EDITOR
        //[NaughtyAttributes.Button]
        public void GenerateAvatars()
        {
            var avatars = _avatarDatabase.Avatars;
            for (int i = 0; i < avatars.Length; i++)
            {
                var card = PrefabUtility.InstantiatePrefab(_cardPefab, _content).GetComponent<AvatarCardUI>();
                card.name = $"Avatar Card UI ({i})";
            }
        }
#endif
    }
}