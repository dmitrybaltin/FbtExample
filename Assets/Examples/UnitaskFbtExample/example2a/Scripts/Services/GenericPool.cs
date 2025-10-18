using System.Collections.Generic;
using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public class GenericPool<TObject, TData> : 
        IGameObjectsPool<TData> where TObject : Object, 
        IPoolable<TData> where TData : class
    {
        private readonly TObject _prefab;
        private readonly Queue<TObject> _pool = new ();
        private readonly HashSet<TObject> _activeObjects = new ();
        private readonly Transform _parent;
        
        public GenericPool(TObject prefab, int initialSize, Transform parent = null)
        {
            this._prefab = prefab;
            this._parent = parent;

            for (var i = 0; i < initialSize; i++)
            {
                TObject obj = CreateNew();
                _pool.Enqueue(obj);
            }
        }

        private TObject CreateNew()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.OnDespawn();
            return obj;
        }

        public IPoolable<TData> Spawn(TData data)
        {
            var obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
            obj.OnSpawn(data);
            obj.OnKillMe += obj1 => Despawn((TObject)obj1);
            _activeObjects.Add(obj);
            return obj;
        }

        public void Despawn(TObject obj)
        {
            if (_activeObjects.Remove(obj))
            {
                obj.OnDespawn();
                _pool.Enqueue(obj);
            }   
            else
            {
                Debug.LogWarning($"Attempting to despawn an object that is not active: {obj.name}");
            }
        }
        
        public void MassDespawn(int count)
        {
            if (count <= 0) return;

            var enumerator = _activeObjects.GetEnumerator();
            int despawned = 0;

            while (despawned < count && enumerator.MoveNext())
            {
                TObject obj = enumerator.Current;
                // удаляем через CopyTo временный массив размера 1
                _activeObjects.Remove(obj);
                obj.OnDespawn();
                _pool.Enqueue(obj);
                despawned++;
                enumerator = _activeObjects.GetEnumerator(); // пересоздаём enumerator после удаления
            }
        }

        public int ActiveCount => _activeObjects.Count;
        
        public int PoolCount => _pool.Count;
    }
}