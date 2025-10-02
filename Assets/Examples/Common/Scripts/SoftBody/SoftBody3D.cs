using UnityEngine;

namespace Baltin.Examples.UnitaskFbt
{
    public class SoftBody3D : MonoBehaviour
    {
        [SerializeField] private Collider centerCollider;
        
        private void Awake()
        {
            if (centerCollider == null)
                return;
            
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var collider1 in colliders)
                foreach (var collider2 in colliders)
                    Physics.IgnoreCollision(collider1, collider2);
            
            /*var bodies = GetComponentsInChildren<Rigidbody2D>();
            foreach (var body in bodies)
            {
                var rb = body.GetComponent<Rigidbody2D>();
                rb.freezeRotation = true;
            }*/
        }
    }

}