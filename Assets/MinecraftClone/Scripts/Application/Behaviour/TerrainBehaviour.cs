using System.Collections;
using UnityEngine;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;
using UniRx;
using UnityEngine.UI;
using System;

namespace MinecraftClone.Application.Behaviour
{
    class TerrainBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject player = null;
        [SerializeField] private DebugScreenBehaviour debugScreen = null;
        [SerializeField] private TerrainService terrainService = null;

        private bool isDrawing;

        void Start()
        {
            if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
            Init();
            SetDebugScreen();

            // Draw
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => !isDrawing)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => Draw())
                .AddTo(gameObject);

            // SetDebugScreen
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => SetDebugScreen())
                .AddTo(gameObject);
        }

        private void Init()
        {
            terrainService.Init(
                terrain: gameObject,
                player: player
            );
            terrainService.DrawAroundPlayer();
            isDrawing = false;
        }

        private void Draw()
        {
            isDrawing = true;

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    terrainService.DrawAroundPlayer(x, z);
                }
            }
            isDrawing = false;
        }

        private void SetDebugScreen()
        {
            debugScreen.currentSeed = terrainService.World.Seed;
            debugScreen.currentChunk = terrainService.CurrentChunk;
            debugScreen.currentBlock = terrainService.CurrentBlock;
            debugScreen.currentPosition = terrainService.CurrentPosition;
        }

        private bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}