using System.Collections.Generic;
using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public class GenericPool<T> where T : MonoBehaviour, IPoolable
    {
        private readonly T prefab;
        private readonly Queue<T> pool = new ();
        private readonly HashSet<T> activeObjects = new ();
        private readonly Transform parent;

        public GenericPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (var i = 0; i < initialSize; i++)
            {
                T obj = CreateNew();
                pool.Enqueue(obj);
            }
        }

        private T CreateNew()
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.OnDespawn();
            return obj;
        }

        public T Spawn()
        {
            T obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();
            obj.OnSpawn();
            activeObjects.Add(obj);
            return obj;
        }

        public void Despawn(T obj)
        {
            if (activeObjects.Remove(obj))
            {
                obj.OnDespawn();
                pool.Enqueue(obj);
            }
            else
            {
                Debug.LogWarning($"Attempting to despawn an object that is not active: {obj.name}");
            }
        }

        public int ActiveCount => activeObjects.Count;
        public int PoolCount => pool.Count;
    }
}