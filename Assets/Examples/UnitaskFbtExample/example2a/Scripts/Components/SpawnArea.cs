using Examples.UnitaskFbtExample.example2a.Scripts.Services;
using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField] private float weight = 1f;

        private BoxCollider _spawnArea;
        private IGameObjectsPool _pool;
        private float _interval;
        private float _timer;
        private Vector3 _center;
        private Vector3 _size;

        public float Weight => weight;

        public void SetPool(IGameObjectsPool pool) => _pool = pool;
        
        public void SetInterval(float interval) => _interval = interval;

        private void Awake()
        {
            _spawnArea = GetComponent<BoxCollider>();

            _center = _spawnArea.bounds.center;
            _size = _spawnArea.bounds.size;
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
                npc.SetPosition(GetRandomPointInZone());
                npc.SetParent(transform);
            }
        }

        private Vector3 GetRandomPointInZone()
        {
            var x = Random.Range(_center.x - _size.x / 2f, _center.x + _size.x / 2f);
            var y = Random.Range(_center.y - _size.y / 2f, _center.y + _size.y / 2f);
            var z = Random.Range(_center.z - _size.z / 2f, _center.z + _size.y / 2f);
            return new Vector3(x, y, z);
        }
    }
}