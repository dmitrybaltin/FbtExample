using UnityEngine;
using UnityEngine.Serialization;

namespace Baltin.Examples.UnitaskFbt
{

    public class SimpleTouchPlayerController2d : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private float kp = 0.5f;
        [SerializeField] private float maxForce = 10f;
        
        private Camera _mainCamera;

        private float _maxForceSquared;

        void Start()
        {
            if(body == null)
                Debug.LogError("Rigidbody2D not set in PlayerController2d");
            
            if(meshRenderer == null)
                Debug.LogError("SkinnedMeshRenderer not set in PlayerController2d");
            
            meshRenderer.material.SetColor("_Color", Color.green);
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            
            _maxForceSquared = maxForce * maxForce;
        }

        void Update()
        {
            if (!TryGetMouseWorldPosition(out var targetPos))
                return;

            var force = (targetPos - body.worldCenterOfMass) * kp;

            var forceMagnitudeSquared = force.sqrMagnitude;
            
            if(forceMagnitudeSquared > _maxForceSquared)
                force *= maxForce/force.magnitude;
            
            body.AddForce(force, ForceMode2D.Impulse);
        }

        private bool TryGetMouseWorldPosition(out Vector2 targetPos)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (ray.direction.z != 0)
            {
                var delta = -ray.origin.z / ray.direction.z;
                targetPos = ray.origin + ray.direction * delta;
                return delta > 0;
            }

            targetPos = Vector2.zero;
            return false;
        }
    }
}