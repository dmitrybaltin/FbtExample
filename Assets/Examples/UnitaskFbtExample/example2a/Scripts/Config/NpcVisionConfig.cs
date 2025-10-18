using System;
using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    [Serializable]
    public class NpcVisionConfig
    {
        public bool enableBatching = true;

        public int raycastsPerNpc = 15;

        public int maxRaycastsPerNpc = 1000;

        public int maxRaycastsPerScene = 100000;
        
        [Range(0f, 360f)] public float viewAngle = 90f;
        
        [Min(0.1f)] public float viewDistance = 10f;
        
        public LayerMask targetMask;
    }
}