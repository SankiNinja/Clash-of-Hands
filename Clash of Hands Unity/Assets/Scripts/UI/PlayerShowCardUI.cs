using ClashOfHands.Systems;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class PlayerShowCardUI : MonoBehaviour, IPoolObject<PlayerShowCardUI>
    {
        public SerializedComponentPool<PlayerShowCardUI> OwnerPool { get; set; }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Free()
        {
            throw new System.NotImplementedException();
        }
    }
}