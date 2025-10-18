using System.Collections.Generic;
using Examples.UnitaskFbtExample.example2a.Scripts.Models;
using UnityEngine;
using Examples.UnitaskFbtExample.example2a.Scripts.Services;
using Tools.AsyncRaycast;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Baltin.UFBT.Example2a
{
    public class SpawnController : MonoBehaviour
    {
        public Example2AConfig config;
        [SerializeField] private UnitaskNpcMonoBehaviour2a npcPrefab;
        [SerializeField] private LateUpdateBatcher physicsBatcher;
        
        public int TargetNpcCount { get; set; }
        public int ActiveNpcCount => _pool.ActiveCount; 
        public int PoolNpcCount => _pool.PoolCount;
        
        [SerializeField] private List<SpawnArea> spawnAreas;
        private float _totalWeight = 0;
        
        [SerializeField] private float maxInterval = 5f;

        [SerializeField] private float proportionalGain = 0.1f;

        private GenericPool<UnitaskNpcMonoBehaviour2a, NpcData> _pool;
        
        private NpcData _npcData;
        
        private void Start()
        {
            // создаем пул на максимальное требуемое количество
            _pool = new GenericPool<UnitaskNpcMonoBehaviour2a, NpcData>(npcPrefab, TargetNpcCount);

            // ищем все дочерние спаун-зоны
            spawnAreas = new List<SpawnArea>(GetComponentsInChildren<SpawnArea>());

            _npcData = new NpcData(config?.npc, physicsBatcher);

            TargetNpcCount = config?.npc.spawn.npcNumber ?? 50;

            // связываем пул со всеми зонами
            foreach (var area in spawnAreas)
            {
                area.SetPool(_pool, _npcData);
                _totalWeight += area.Weight;
            }
        }

        private void Update()
        {
            AdjustSpawnIntervals();
        }

        private float _lastKillTime;
        
        private void AdjustSpawnIntervals()
        {
            //П-регулятор частоты спауна/деспауна ботов
            var spawnRate = proportionalGain * (TargetNpcCount - _pool.ActiveCount);
            
            if (spawnRate > 0)
                Spawn(spawnRate);
            else
                Despawn(spawnRate);
        }

        private void Spawn(float spawnRate)
        {
            if (_totalWeight <= 0f) return;            

            var relatedSpawnRate = spawnRate / _totalWeight;
            
            for (var i = 0; i < spawnAreas.Count; i++)
            {
                var area = spawnAreas[i];
                var rate = relatedSpawnRate * area.Weight;

                if (rate > Mathf.Epsilon)
                    area.SetInterval(1 / rate);
            }
        }
        
        private void Despawn(float spawnRate)
        {
            if (spawnRate > -Mathf.Epsilon)
                return;
                
            var killInterval = -1 / spawnRate;

            if (Time.time - _lastKillTime >=  killInterval)
            {
                var npcToKill = (Time.time - _lastKillTime) / killInterval;
                _pool.MassDespawn((int)npcToKill);
                _lastKillTime = Time.time;
            }
        }
    }
}