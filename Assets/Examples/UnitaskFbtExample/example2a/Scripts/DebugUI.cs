using Baltin.UFBT.Example2a;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.UnitaskFbtExample.example2a
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField] private Slider npcCountSlider;
        [SerializeField] private Slider raycastsPerNpcSlider;

        [SerializeField] private Example2AConfig config;
        /*
        private void Start()
        {
            npcCountSlider.onValueChanged.AddListener(OnNpcCountChanged);
            raycastsPerNpcSlider.onValueChanged.AddListener(OnNpcCountChanged);

            if (config is null)
                return;
            
            npcCountSlider.maxValue = config.npc.spawn.maxNpcNumber;
            raycastsPerNpcSlider.maxValue = config.npc.vision.maxRaycastsPerNpc;

            npcCountSlider.value = config.npc.spawn.npcNumber;
            raycastsPerNpcSlider.value = config.npc.vision.raycastsPerNpc;
        }

        private void OnDestroy()
        {
            npcCountSlider.onValueChanged.RemoveListener(OnNpcCountChanged);
            raycastsPerNpcSlider.onValueChanged.RemoveListener(OnRaycastsPerNpcSlider);
        }
    
        private void OnNpcCountChanged(float value)
        {
            config.npc.spawn.npcNumber = (int)value;
        }
    
        private void OnRaycastsPerNpcSlider(float value)
        {
            config.npc.vision.raycastsPerNpc = (int)value;
        }*/
    }
}