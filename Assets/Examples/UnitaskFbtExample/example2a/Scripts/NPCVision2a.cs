using System.Collections.Generic;
using Baltin.UFBT.Example2a.Abstraction;
using Cysharp.Threading.Tasks;
using Tools.AsyncRaycast.Abstraction;
using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    public class NpcVision2a : INpcVision
    {
        private Vector3[] _directions;
        private IPhysicsBatcher _physicsBatcher;
        private NpcVisionConfig _config;

        public NpcVision2a(NpcVisionConfig config, IPhysicsBatcher physicsBatcher)
        {
            _config = config;
            _physicsBatcher = physicsBatcher;
        }
        
        /// <summary>
        /// Async search using async RaycastCommand
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public async UniTask<Transform> FindTargetAsync(Transform origin)
        {
            if (_physicsBatcher is null)
                return FindTarget(origin);
              
            RecalculateDirections();
            
            var hit = await _physicsBatcher.RaycastAsync(RaycastEnumerator(origin));

            if (hit.collider == null) 
                return null;
            
            return hit.collider.gameObject.transform;
        }

        /// <summary>
        /// Sync search using Physics.Raycast()
        /// </summary>
        public Transform FindTarget(Transform origin)
        {
            RecalculateDirections();
            
            foreach (var dir in _directions)
                if (Physics.Raycast(
                        origin.position, 
                        origin.rotation * dir, 
                        out var hit, 
                        _config.viewDistance, 
                        _config.targetMask))
                    return hit.collider.gameObject.transform;

            return null;
        }

        public void DrawDebug(Transform origin)
        {
            if (_physicsBatcher is null)
                return;
            
            {
                var dir = _directions[_directions.Length / 2];
                var direction = origin.rotation * dir * _config.viewDistance;
                Debug.DrawRay(origin.position, direction, Color.red);
            }

            if (_directions == null) return;

            foreach (var dir in _directions)
            {
                var direction = origin.rotation * dir * _config.viewDistance;
                Debug.DrawRay(origin.position, direction, Color.yellow);
            }
        }

        private void RecalculateDirections()
        {
            if (_directions is not null && _directions.Length == _config.raycastsPerNpc)
                return;
            
            _directions = new Vector3[_config.raycastsPerNpc];
            var halfAngle = _config.viewAngle * 0.5f;

            for (var i = 0; i < _directions.Length; i++)
            {
                var t = _directions.Length == 1 ? 0.5f : (float)i / (_directions.Length - 1); // For the first ray be at the center
                var angle = Mathf.Lerp(-halfAngle, halfAngle, t);

                var rot = Quaternion.AngleAxis(angle, Vector3.forward);
                _directions[i] = rot * Vector3.up;
            }
        }
        
        private IEnumerable<RaycastCommand> RaycastEnumerator(Transform origin)
        {
            foreach (var dir in _directions)
            {
                var command = new RaycastCommand(
                    origin.position, 
                    origin.rotation * dir, 
                    _config.viewDistance, 
                    _config.targetMask);

                yield return command;
            }
        }


    }
}