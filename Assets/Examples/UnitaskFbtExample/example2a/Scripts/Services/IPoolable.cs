using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public interface IPoolable<TData> where TData : class
    {
        void OnSpawn(TData data);
        
        void OnDespawn();
        
        event System.Action<IPoolable<TData>> OnKillMe;

        public void SetPosition(Vector3 position);

        public void SetParent(Transform parent);

    }
}