using UnityEngine;
using UnityEngine.Assertions;

namespace ClashOfHands.Systems
{
    public class MonoBehaviourSingleton<TBehaviour> : MonoBehaviour where TBehaviour : MonoBehaviour
    {
        private static TBehaviour _instance;

        public static TBehaviour Instance
        {
            get
            {
                Assert.IsNotNull(_instance,
                    $"Missing Singleton Instance {nameof(TBehaviour)}: There is no object in the scene with the component {nameof(TBehaviour)}. Please add the required component of the type to the game object.");
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError(
                    $"Multiple Singleton Instances of {nameof(TBehaviour)} : Make sure there is only one instance of an object with this component in the scene.",
                    this);
            }

            _instance = this as TBehaviour;
        }
    }
}