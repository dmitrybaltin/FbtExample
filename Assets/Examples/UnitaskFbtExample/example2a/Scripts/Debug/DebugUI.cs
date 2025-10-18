using Baltin.UFBT.Example2a;
using Tools.AsyncRaycast;
using Tools.DebugUI;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.UnitaskFbtExample.example2a
{
    public class DebugUI : MonoBehaviour
    {
        public NpcConfig  npcConfig;
        
        [SerializeField] private LateUpdateBatcher batcher;

        [SerializeField] private SpawnController spawnController;

        private void Update()
        {
            if (batcher is not null)
            {
                Dbg.Display("", "Raycasts", batcher.LastBatchCount);
                Dbg.Display("", "Queue", batcher.QueueCount);
            }

            if (spawnController is not null)
            {
                Dbg.Display("", "ActiveNpcCount", spawnController.ActiveNpcCount);
                Dbg.Display("", "PoolNpcCount", spawnController.PoolNpcCount);
                Dbg.Display("", "TargetNpcCount", spawnController.TargetNpcCount);
                Dbg.Slider("", "TargetNpcCount",
                    spawnController.TargetNpcCount,
                    value => spawnController.TargetNpcCount = (int)value,
                    1,
                    1000);
            }

            if (npcConfig is not null)
            {
                Dbg.Slider("", "EnableBatching",
                    npcConfig.vision.enableBatching ? 1 : 0,
                    value => npcConfig.vision.enableBatching = value > 0.5f,
                    0,
                    1);
            }
        }
    }
}