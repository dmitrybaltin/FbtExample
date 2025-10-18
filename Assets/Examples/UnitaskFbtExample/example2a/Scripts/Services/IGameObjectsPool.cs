namespace Examples.UnitaskFbtExample.example2a.Scripts.Services
{
    public interface IGameObjectsPool<TData> where TData : class
    {
        IPoolable<TData> Spawn(TData data);
        int ActiveCount { get; }
        int PoolCount { get; }
        void MassDespawn(int count);
    }
}