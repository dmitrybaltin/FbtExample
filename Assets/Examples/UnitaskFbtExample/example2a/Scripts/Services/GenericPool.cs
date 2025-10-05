using System.Collections.Generic;
using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public class GenericPool<T> : IGameObjectsPool where T : Object, IPoolable
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

        public IPoolable Spawn()
        {
            T obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();
            obj.OnSpawn();
            obj.OnKillMe += obj1 => Despawn((T)obj1);
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

        //Despawn given quantity of objects
        public void MassDespawnOld(int count)
        {
            if (count <= 0) return;

            var enumerator = activeObjects.GetEnumerator();
            var despawned = 0;

            while (despawned < count && enumerator.MoveNext())
            {
                T obj = enumerator.Current;
                // напрямую удаляем из HashSet и возвращаем в пул без повторного поиска
                if (activeObjects.Remove(obj))
                {
                    obj.OnDespawn();
                    pool.Enqueue(obj);
                    despawned++;
                }
            }
        }
        
        public void MassDespawn(int count)
        {
            if (count <= 0) return;

            var enumerator = activeObjects.GetEnumerator();
            int despawned = 0;

            while (despawned < count && enumerator.MoveNext())
            {
                T obj = enumerator.Current;
                // удаляем через CopyTo временный массив размера 1
                activeObjects.Remove(obj);
                obj.OnDespawn();
                pool.Enqueue(obj);
                despawned++;
                enumerator = activeObjects.GetEnumerator(); // пересоздаём enumerator после удаления
            }
        }


        public int ActiveCount => activeObjects.Count;
        
        public int PoolCount => pool.Count;
    }
}