using UnityEngine;

namespace ClashOfHands.Systems
{
    public class Player : MonoBehaviour
    {
        public int PlayerIndex { get; private set; }
        public int AvatarId { get; private set; }

        public int HighScore { get; set; }

        public int Hearts { get; set; }

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
            PlayerPrefs.SetInt(nameof(HighScore), HighScore);
        }

        public void LoadPlayerData()
        {
            if (PlayerPrefs.HasKey(nameof(AvatarId)))
                AvatarId = PlayerPrefs.GetInt(nameof(AvatarId));
            else
                AvatarId = 0;

            if (PlayerPrefs.HasKey(nameof(HighScore)))
                HighScore = PlayerPrefs.GetInt(nameof(HighScore));
            else
                HighScore = 0;
        }
    }
}