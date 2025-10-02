using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    [CreateAssetMenu(fileName = "Example2aConfig", menuName = "FbtExamples/Example2aConfig", order = 0)]
    public class Example2AConfig : ScriptableObject
    {
        public NpcConfig npc;
    }
}