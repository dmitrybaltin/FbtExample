using System;
using Baltin.UFBT.Example2a;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Examples.UnitaskFbtExample.example2a.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Example2AConfig config;

        [SerializeField] private SpawnController spawnController;
        
        [SerializeField] private DebugUI debugUI;
        
        private void Awake()
        {
            spawnController.config = config;
            debugUI.npcConfig = config.npc;
        }
    }
}