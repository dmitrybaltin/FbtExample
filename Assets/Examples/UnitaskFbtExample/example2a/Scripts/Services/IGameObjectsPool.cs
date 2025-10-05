namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public interface IGameObjectsPool
    {
        IPoolable Spawn();
        int ActiveCount { get; }
        int PoolCount { get; }
        void MassDespawn(int count);
    }
}