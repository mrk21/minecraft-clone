using System;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Store;
using System.Threading.Tasks;

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
                .Subscribe(async _ => await Draw())
                .AddTo(gameObject);

            // PutBlock
            player.OnPut
                .Subscribe(async _ => await PutBlock())
                .AddTo(gameObject);

            // RemoveBlock
            player.OnRemove
                .Subscribe(async _ => await RemoveBlock())
                .AddTo(gameObject);
        }

        private void JoinDimension(Dimension dimension)
        {
            view.Init(dimension);
            player.JoinDimension(dimension);
        }

        private async Task Draw()
        {
            await view.DrawArroundChunk(player.CurrentChunk.Value);
        }

        private async Task PutBlock()
        {
            await view.PutBlock(player.Gaze.Value, player.OperationRange.Value);
        }

        private async Task RemoveBlock()
        {
            await view.RemoveBlock(player.Gaze.Value, player.OperationRange.Value);
        }
    }
}
