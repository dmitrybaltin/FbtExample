using Examples.UnitaskFbtExample.example2a.Scripts.Services;
using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    [RequireComponent(typeof(BoxCollider))]
    public class NpcSpawnArea : MonoBehaviour
    {
        [SerializeField] private float weight = 1f;

        private BoxCollider _spawnArea;
        private GenericPool<UnitaskNpcMonoBehaviour2a> _pool;
        private float _interval;
        private float _timer;

        public float Weight => weight;

        public void SetPool(GenericPool<UnitaskNpcMonoBehaviour2a> pool) => _pool = pool;
        public void SetInterval(float interval) => _interval = interval;

        private void Awake()
        {
            _spawnArea = GetComponent<BoxCollider>();
            // отключаем коллайдер, чтобы он не участвовал в физике
            _spawnArea.enabled = false;
        }

        private void Update()
        {
            if (_pool is null || _interval <= 0f)
                return;

            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                _timer = 0f;
                Spawn(1);
            }
        }

        private void Spawn(int count)
        {
            if (_pool is null)
                return;

            for (var i = 0; i < count; i++)
            {
                var npc = _pool.Spawn();
                npc.transform.position = GetRandomPointInZone();
                npc.transform.parent = transform;
            }
        }

        private Vector3 GetRandomPointInZone()
        {
            var center = _spawnArea.bounds.center;
            var size = _spawnArea.bounds.size;
            var x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
            var y = center.y;
            var z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);
            return new Vector3(x, y, z);
        }
    }
}