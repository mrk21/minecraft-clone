using System;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class DebugScreenPresenter : MonoBehaviour
    {
        private DebugScreenView view;

        private GameProgress gameProgress;
        private Player player;
        private PlaySetting playSetting;

        void Awake()
        {
            view = GetComponent<DebugScreenView>();
            gameProgress = GameProgress.Get();
            player = gameProgress.CurrentWorld.Value.Player.Value;
            playSetting = gameProgress.CurrentWorld.Value.PlaySetting.Value;
        }

        void Start()
        {
            // SetEnabled()
            Observable
                .Merge(
                    playSetting.IsEnabledDebugScreen.AsUnitObservable(),
                    gameProgress.WorldIsActivated.AsUnitObservable()
                )
                .BatchFrame(0, FrameCountType.Update)
                .Subscribe(_ => SetEnabled())
                .AddTo(gameObject);

            // ToggleDebugScreen
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.WorldIsActivated.Value)
                .Where(_ => Input.GetKeyDown(KeyCode.F2))
                .Subscribe(_ => ToggleDebugScreen())
                .AddTo(gameObject);

            // Display()
            Observable.Merge(
                player.Position.AsUnitObservable(),
                player.CurrentChunk.AsUnitObservable(),
                player.CurrentBlock.AsUnitObservable()
            )
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .BatchFrame(0, FrameCountType.Update)
                .Subscribe(_ => Display())
                .AddTo(gameObject);
        }

        private void Display()
        {
            view.Display(
                currentSeed: player.CurrentDimension.Value.Seed,
                currentChunk: player.CurrentChunk.Value,
                currentBlock: player.CurrentBlock.Value,
                currentPosition: player.Position.Value
            );
        }

        private void SetEnabled()
        {
            var enabled = playSetting.IsEnabledDebugScreen.Value && gameProgress.WorldIsActivated.Value;
            view.SetEnabled(enabled);
        }

        private void ToggleDebugScreen()
        {
            playSetting.IsEnabledDebugScreen.Value = !playSetting.IsEnabledDebugScreen.Value;
        }
    }
}
