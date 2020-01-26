using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class CursorScreenPresenter : MonoBehaviour
    {
        private GameProgress gameProgress = null;
        private PlaySetting playSetting = null;

        void Start()
        {
            gameProgress = GameProgress.Get();
            playSetting = gameProgress.CurrentWorld.Value.PlaySetting.Value;

            // SetEnabled()
            Observable
                .Merge(
                    playSetting.Camera.AsUnitObservable(),
                    gameProgress.WorldIsActivated.AsUnitObservable()
                )
                .BatchFrame(0, FrameCountType.Update)
                .Subscribe(_ => SetEnabled())
                .AddTo(gameObject);
        }

        private void SetEnabled()
        {
            var enabled = playSetting.Camera.Value == PlaySetting.CameraType.main && gameProgress.WorldIsActivated.Value;
            gameObject.SetActive(enabled);
        }
    }
}
