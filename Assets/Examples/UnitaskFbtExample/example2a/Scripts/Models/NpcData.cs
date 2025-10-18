using Baltin.UFBT.Example2a;
using Tools.AsyncRaycast.Abstraction;

namespace Examples.UnitaskFbtExample.example2a.Scripts.Models
{
    public class NpcData
    {
        public NpcConfig Config;
        public IPhysicsBatcher Batcher;

        public NpcData(NpcConfig config, IPhysicsBatcher batcher)
        {
            Config = config;
            Batcher = batcher;
        }
    }
}