using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public interface IPoolable
    {
        void OnSpawn();
        
        void OnDespawn();
        
        GameObject gameObject { get; }
    }
}