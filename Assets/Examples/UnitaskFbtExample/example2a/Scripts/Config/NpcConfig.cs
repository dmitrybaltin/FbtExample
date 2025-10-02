using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Baltin.UFBT.Example2a
{
    [Serializable]
    public class NpcConfig
    {
        [SerializeField] public NpcSpawnConfig spawn;
        
        [SerializeField] public NpcVisionConfig vision;
        
        /// <summary>
        /// Coefficient to a gravity force between the NPC and the player when them are close to each other  
        /// </summary>
        [SerializeField] public float attackForce = 10f;

        /// <summary>
        /// Coefficientto a gravity force between the NPC and the player when them are not close to each other  
        /// </summary>
        [SerializeField] public float movingForce = 1f;

        [SerializeField] public float rangePreparingDuration = 1f;

        [SerializeField] public float rangeAttackDuration = 2f;

        [SerializeField] public float patrolForce = 0.2f;

        [SerializeField] public float patrolTorque = 1f;
        
        public float relaxDuration = 3f;
    }
}