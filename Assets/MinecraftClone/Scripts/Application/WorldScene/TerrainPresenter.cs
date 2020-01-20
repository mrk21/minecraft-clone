﻿using System;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    class TerrainPresenter : MonoBehaviour
    {
        private TerrainView view;
        private Player player;
        private GameProgress gameProgress;

        void Start()
        {
            view = GetComponent<TerrainView>();
            player = Singleton<Player>.Instance;
            gameProgress = Singleton<GameProgress>.Instance;

            // JoinWorld
            gameProgress.currentDimension
                .Where(dimension => dimension != null)
                .Subscribe(JoinDimension)
                .AddTo(gameObject);
            
            // Draw
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => !view.isDrawing.Value)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => Draw())
                .AddTo(gameObject);

            // PutBlock
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetMouseButtonDown(1))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => PutBlock())
                .AddTo(gameObject);

            // RemoveBlock
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetMouseButtonDown(0))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => RemoveBlock())
                .AddTo(gameObject);
        }

        private void JoinDimension(Dimension dimension)
        {
            view.Init(dimension);
            player.JoinDimension(dimension);
        }

        private void Draw()
        {
            view.DrawArroundChunk(player.CurrentChunk());
        }

        private void PutBlock()
        {
            view.PutBlock(player.gaze.Value, player.operationRange.Value);
        }

        private void RemoveBlock()
        {
            view.RemoveBlock(player.gaze.Value, player.operationRange.Value);
        }
    }
}