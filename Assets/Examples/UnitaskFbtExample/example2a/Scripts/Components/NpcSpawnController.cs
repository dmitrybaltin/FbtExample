using System.Collections.Generic;
using UnityEngine;
using Examples.UnitaskFbtExample.example2a.Scripts.Services;

namespace Baltin.UFBT.Example2a
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private UnitaskNpcMonoBehaviour2a npcPrefab;
        [SerializeField] private int targetNpcCount = 50;
        [SerializeField] private List<NpcSpawnArea> spawnAreas;

        [SerializeField] private float minInterval = 0.1f;
        [SerializeField] private float maxInterval = 5f;
        [SerializeField] private float proportionalGain = 0.5f;

        private GenericPool<UnitaskNpcMonoBehaviour2a> _pool;

        private void Awake()
        {
            // создаем пул на максимальное требуемое количество
            _pool = new GenericPool<UnitaskNpcMonoBehaviour2a>(npcPrefab, targetNpcCount);

            // ищем все дочерние спаун-зоны
            spawnAreas = new List<NpcSpawnArea>(GetComponentsInChildren<NpcSpawnArea>());

            // связываем пул со всеми зонами
            foreach (var area in spawnAreas)
            {
                area.SetPool(_pool);
            }
        }


        private void Update()
        {
            AdjustSpawnIntervals();
        }

        private void AdjustSpawnIntervals()
        {
            var currentNpcCount = _pool.ActiveCount;
            var error = targetNpcCount - currentNpcCount;

            // простой P-контроллер: управляющее воздействие
            var baseInterval = Mathf.Clamp(maxInterval - proportionalGain * error, minInterval, maxInterval);

            // вычисляем суммарный вес
            var totalWeight = 0f;
            foreach (var area in spawnAreas)
                totalWeight += area.Weight;

            // устанавливаем интервалы для каждой зоны с учетом веса
            foreach (var area in spawnAreas)
            {
                var weightFraction = area.Weight / totalWeight;
                var interval = baseInterval / weightFraction; // зоны с большим весом спавнят быстрее
                area.SetInterval(interval);
            }
        }
    }
}