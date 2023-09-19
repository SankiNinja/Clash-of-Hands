using UnityEngine;

namespace ClashOfHands.Systems
{
    public class Player : MonoBehaviour
    {
        public int PlayerIndex { get; private set; }
        public int AvatarId { get; private set; }

        public Sprite AvatarSprite { get; private set; }

        public void SetPlayerIndex(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public void SetAvatar(int index, Sprite sprite)
        {
            AvatarId = index;
            AvatarSprite = sprite;
        }

        public void SaveData()
        {
            PlayerPrefs.SetInt(nameof(AvatarId), AvatarId);
        }

        public bool LoadPlayerData()
        {
            var hasData = PlayerPrefs.HasKey(nameof(AvatarId));
            if (hasData)
                AvatarId = PlayerPrefs.GetInt(nameof(AvatarId));

            return hasData;
        }
    }
}