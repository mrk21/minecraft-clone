using System;
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
        private GameProgress gameProgress;
        private World world;
        private Player player;

        void Start()
        {
            view = GetComponent<TerrainView>();
            gameProgress = GameProgress.Get();
            world = gameProgress.CurrentWorld.Value;
            player = world.Player.Value;

            // JoinWorld
            world.CurrentDimension
                .Where(dimension => dimension != null)
                .Subscribe(JoinDimension)
                .AddTo(gameObject);
            
            // Draw
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => !view.isDrawing.Value)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => Draw())
                .AddTo(gameObject);

            // PutBlock
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetMouseButtonDown(1))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => PutBlock())
                .AddTo(gameObject);

            // RemoveBlock
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
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
            view.DrawArroundChunk(player.CurrentChunk.Value);
        }

        private void PutBlock()
        {
            view.PutBlock(player.Gaze.Value, player.OperationRange.Value);
        }

        private void RemoveBlock()
        {
            view.RemoveBlock(player.Gaze.Value, player.OperationRange.Value);
        }
    }
}
