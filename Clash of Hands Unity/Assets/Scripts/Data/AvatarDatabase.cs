using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Avatar Database", fileName = "Avatar_Database_")]
public class AvatarDatabase : ScriptableObject
{
    [SerializeField]
    public Sprite[] Avatars;

    public Sprite GetRandomAvatar(out int index)
    {
        index = Random.Range(0, Avatars.Length);
        return Avatars[index];
    }
}