using System.Collections.Generic;
using UnityEngine;
using Examples.UnitaskFbtExample.example2a.Scripts.Services;

namespace Baltin.UFBT.Example2a
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private UnitaskNpcMonoBehaviour2a npcPrefab;
        [SerializeField][Range(0,1000)] private int targetNpcCount = 100;
        [SerializeField] private List<SpawnArea> spawnAreas;
        private float _totalWeight = 0;
        
        [SerializeField] private float maxInterval = 5f;

        [SerializeField] private float proportionalGain = 0.1f;

        private GenericPool<UnitaskNpcMonoBehaviour2a> _pool;

        private void Awake()
        {
            // создаем пул на максимальное требуемое количество
            _pool = new GenericPool<UnitaskNpcMonoBehaviour2a>(npcPrefab, targetNpcCount);

            // ищем все дочерние спаун-зоны
            spawnAreas = new List<SpawnArea>(GetComponentsInChildren<SpawnArea>());

            // связываем пул со всеми зонами
            foreach (var area in spawnAreas)
            {
                area.SetPool(_pool);
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
            var spawnRate = proportionalGain * (targetNpcCount - _pool.ActiveCount);
            
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