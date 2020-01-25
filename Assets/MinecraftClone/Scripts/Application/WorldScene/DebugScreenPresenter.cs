using System;
using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class DebugScreenPresenter : MonoBehaviour
    {
        private DebugScreenView view;

        private GameProgress gameProgress;
        private Player player;
        private PlaySetting playSetting;
        
        void Start()
        {
            view = GetComponent<DebugScreenView>();

            gameProgress = GameProgress.Get();
            player = gameProgress.CurrentWorld.Player;
            playSetting = gameProgress.CurrentWorld.PlaySetting;

            // SetEnabled()
            Observable
                .Merge(
                    playSetting.isEnabledDebugScreen.AsUnitObservable(),
                    gameProgress.worldIsActivated.AsUnitObservable()
                )
                .BatchFrame(0, FrameCountType.Update)
                .Subscribe(_ => SetEnabled())
                .AddTo(gameObject);

            // ToggleDebugScreen
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.worldIsActivated.Value)
                .Where(_ => Input.GetKeyDown(KeyCode.F2))
                .Subscribe(_ => ToggleDebugScreen())
                .AddTo(gameObject);

            // Display()
            player.position
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .BatchFrame(0, FrameCountType.Update)
                .Subscribe(_ => Display())
                .AddTo(gameObject);
        }

        private void Display()
        {
            view.Display(
                currentSeed: player.currentDimension.Value.Seed,
                currentChunk: player.CurrentChunk(),
                currentBlock: player.CurrentBlock(),
                currentPosition: player.position.Value
            );
        }

        private void SetEnabled()
        {
            var enabled = playSetting.isEnabledDebugScreen.Value && gameProgress.worldIsActivated.Value;
            view.SetEnabled(enabled);
        }

        private void ToggleDebugScreen()
        {
            playSetting.isEnabledDebugScreen.Value = !playSetting.isEnabledDebugScreen.Value;
        }
    }
}
