using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baltin.Examples.UnitaskFbt
{
    public class SimpleKeyboardPlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _mainRigidbody;
        [SerializeField] private float mainBodyForce = 10f; // сила, с которой толкаем
        [SerializeField] private float otherBodiesForce = 10f; // сила, с которой толкаем

        private List<Rigidbody> _rigidbodies;
        private void Awake()
        {
            InitializeRigidbodies();
        }

        private void FixedUpdate() // физика только в FixedUpdate
        {
            var h = Input.GetAxis("Horizontal"); // A/D или стрелки ← →
            var v = Input.GetAxis("Vertical");   // W/S или стрелки ↑ ↓

            Vector3 direction = new Vector3(h, 0, v);

            _mainRigidbody.AddForce(direction * mainBodyForce, ForceMode.VelocityChange);
            foreach (var rb in _rigidbodies)
                rb.AddForce(direction * otherBodiesForce, ForceMode.VelocityChange);
        }

        private void InitializeRigidbodies()
        {
            var allRigidbodies = GetComponentsInChildren<Rigidbody>();

            if (allRigidbodies is null || allRigidbodies.Length == 0)
                throw new NullReferenceException("No one rigidbody attached to the SimpleKeyboardPlayerController");

            _rigidbodies = allRigidbodies.ToList();
            
            if(_mainRigidbody is not null && _rigidbodies.Count > 0)
                for (int i = 0; i < _rigidbodies.Count; i++)
                    if (_rigidbodies[i].GetInstanceID() == _mainRigidbody.GetInstanceID())
                    {
                        _rigidbodies.RemoveAt(i);
                        return;
                    }
        }
    }
}