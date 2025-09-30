using Cysharp.Threading.Tasks;
using Tools.AsyncRaycast.Abstraction;
using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    public class NpcVision2a : INpcVision
    {
        private readonly Vector3[] _directions;
        private readonly float _viewDistance;
        private readonly LayerMask _targetMask;
        private IPhysicsBatcher _physicsBatcher;

        public NpcVision2a(IPhysicsBatcher physicsBatcher, float viewAngle, int rayCount, float viewDistance, LayerMask targetMask)
        {
            _physicsBatcher = physicsBatcher;
            
            _viewDistance = viewDistance;
            _targetMask = targetMask;

            // Вычисляем направления один раз
            _directions = new Vector3[rayCount];
            var halfAngle = viewAngle * 0.5f;

            for (var i = 0; i < rayCount; i++)
            {
                var t = (rayCount == 1) ? 0.5f : (float)i / (rayCount - 1); // чтобы 1 луч оказался по центру
                var angle = Mathf.Lerp(-halfAngle, halfAngle, t);

                Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
                _directions[i] = rot * Vector3.up; // базовое направление "вверх"
            }
        }

        /// <summary>
        /// Find a target
        /// </summary>
        public async UniTask<Transform> FindTargetAsync(Transform origin)
        {
            if (_physicsBatcher is null) 
                return null;
            
            var dir = _directions[_directions.Length/2];
            
            var direction = origin.rotation * dir;
            var distance = _viewDistance;
            var layerMask = _targetMask;

            var command = new RaycastCommand(origin.position, direction, distance, layerMask);

            var hit = await _physicsBatcher.RaycastAsync(command);

            if (hit.collider == null) 
                return null;
            
            return hit.collider.gameObject.transform;
        }
        
        /// <summary>
        /// Find a target
        /// </summary>
        public Transform FindTarget(Transform origin)
        {
            foreach (var dir in _directions)
                if (Physics.Raycast(origin.position, origin.rotation * dir, out RaycastHit hit, _viewDistance, _targetMask))
                    return hit.collider.gameObject.transform;

            return null;
        }

        public void DrawDebug(Transform origin)
        {
            {
                var dir = _directions[_directions.Length / 2];
                var direction = origin.rotation * dir * _viewDistance;
                Debug.DrawRay(origin.position, direction, Color.red);
            }

            if (_directions == null) return;

            foreach (var dir in _directions)
            {
                var direction = origin.rotation * dir * _viewDistance;
                Debug.DrawRay(origin.position, direction, Color.yellow);
            }
        }
    }
}