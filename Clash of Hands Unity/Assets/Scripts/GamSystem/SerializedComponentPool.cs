using System.Collections.Generic;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public interface IPoolObject<TComponent> where TComponent : MonoBehaviour, IPoolObject<TComponent>
    {
        SerializedComponentPool<TComponent> OwnerPool { set; }
        void Clear();
        void Free();
    }

    public class SerializedComponentPool<TComponent> : MonoBehaviour where TComponent : MonoBehaviour, IPoolObject<TComponent>
    {
        [SerializeField]
        private List<TComponent> _pool;

        [SerializeField]
        private TComponent _componentPrefab;

        private void Start()
        {
            foreach (var component in _pool)
            {
                component.gameObject.SetActive(false);
                component.Clear();
            }
        }

        public TComponent Get()
        {
            var lasIndex = _pool.Count - 1;
            var component = lasIndex < 0 ? Instantiate(_componentPrefab, transform) : _pool[^1];
            component.OwnerPool = this;
            _pool.RemoveAt(lasIndex);
            return component;
        }

        public void Release(TComponent pooledObject)
        {
            _pool.Add(pooledObject);

            pooledObject.gameObject.SetActive(false);
            pooledObject.Clear();
        }
    }
}