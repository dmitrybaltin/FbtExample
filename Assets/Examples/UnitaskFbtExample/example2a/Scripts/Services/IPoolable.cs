using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public interface IPoolable
    {
        void OnSpawn();
        
        void OnDespawn();
        
        event System.Action<IPoolable> OnKillMe;

        public void SetPosition(Vector3 position);

        public void SetParent(Transform parent);

    }
}