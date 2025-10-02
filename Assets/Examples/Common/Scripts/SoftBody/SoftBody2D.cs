using UnityEngine;

namespace Baltin.Examples.UnitaskFbt
{
    public class SoftBody : MonoBehaviour
    {
        [SerializeField] private Collider2D centerCollider;
        
        private void Awake()
        {
            if (centerCollider == null)
                return;
            
            var colliders = GetComponentsInChildren<Collider2D>();
            foreach (var collider1 in colliders)
                foreach (var collider2 in colliders)
                    Physics2D.IgnoreCollision(collider1, collider2);
            
            /*var bodies = GetComponentsInChildren<Rigidbody2D>();
            foreach (var body in bodies)
            {
                var rb = body.GetComponent<Rigidbody2D>();
                rb.freezeRotation = true;
            }*/
        }
    }

}