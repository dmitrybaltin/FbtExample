using UnityEngine;

namespace Baltin.UFBT.Example2
{
    public class NpcVision
    {
        private readonly Vector3[] _directions;
        private readonly float _viewDistance;
        private readonly LayerMask _targetMask;

        public NpcVision(float viewAngle, int rayCount, float viewDistance, LayerMask targetMask)
        {
            this._viewDistance = viewDistance;
            this._targetMask = targetMask;

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
        public Transform FindTarget(Transform origin)
        {
            foreach (var dir in _directions)
                if (Physics.Raycast(origin.position, origin.rotation * dir, out RaycastHit hit, _viewDistance, _targetMask))
                    return hit.collider.gameObject.transform;

            return null;
        }

        public void DrawDebug(Transform origin)
        {
            if (_directions == null) return;

            foreach (var dir in _directions)
            {
                var direction = origin.rotation * dir * _viewDistance;
                Debug.DrawRay(origin.position, direction, Color.yellow);
            }
        }
    }
}