using UnityEngine;
using UnityEngine.UI;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain;
using System;
using UniRx;

namespace MinecraftClone.Application.Behaviour
{
    public class DebugScreenBehaviour : MonoBehaviour
    {
        public Seed currentSeed;
        public Chunk currentChunk;
        public BaseBlock currentBlock;
        public Vector3 currentPosition;

        void Start()
        {
            GetComponent<Text>().text = "";

            // Display
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => Display())
                .AddTo(gameObject);
        }

        void Display()
        {
            string text = "";
            if (currentSeed != null)
            {
                text += $"CurrentSeed: {currentSeed.Base}\n";
            }
            if (currentChunk != null)
            {
                text += $"CurrentCunk: {currentChunk.Address}\n";
            }
            if (currentBlock != null)
            {
                text += $"CurrentBlock: {currentBlock}\n";
            }
            if (currentPosition != null)
            {
                text += $"CurrentPosition: {currentPosition}\n";
            }
            GetComponent<Text>().text = text;
        }

        bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}