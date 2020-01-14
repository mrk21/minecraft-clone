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
        public GameObject player = null; // set by the inspector
        public GameObject debugScreen = null; // set by the inspector
        public TerrainService terrainService;
        public bool isDrawing;

        void Start()
        {
            if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
            Init();
            SetDebugScreen();

            var drawingStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => !isDrawing)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => Draw());

            var settingDebugScreenStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => SetDebugScreen());

            var initingStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.R))
                .ThrottleFirst(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ => Init());

            var redrawingStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.P))
                .ThrottleFirst(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ => Redraw());
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

        private void Redraw()
        {
            isDrawing = true;
            terrainService.Redraw();
            isDrawing = false;
        }

        private void SetDebugScreen()
        {
            debugScreen.GetComponent<DebugScreenBehaviour>().currentSeed = terrainService.World.Seed;
            debugScreen.GetComponent<DebugScreenBehaviour>().currentChunk = terrainService.CurrentChunk;
            debugScreen.GetComponent<DebugScreenBehaviour>().currentBlock = terrainService.CurrentBlock;
            debugScreen.GetComponent<DebugScreenBehaviour>().currentPosition = terrainService.CurrentPosition;
        }

        private bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}