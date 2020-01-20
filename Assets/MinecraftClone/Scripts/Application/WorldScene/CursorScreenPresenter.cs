using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class CursorScreenPresenter : MonoBehaviour
    {
        private PlaySetting playSetting = null;
        private GameProgress gameProgress = null;

        void Start()
        {
            playSetting = Singleton<PlaySetting>.Instance;
            gameProgress = Singleton<GameProgress>.Instance;

            // SetEnabled()
            Observable
                .Merge(
                    playSetting.cameraType.AsUnitObservable(),
                    gameProgress.worldIsActivated.AsUnitObservable()
                )
                .BatchFrame(0, FrameCountType.Update)
                .Subscribe(_ => SetEnabled())
                .AddTo(gameObject);
        }

        private void SetEnabled()
        {
            var enabled = playSetting.cameraType.Value == PlaySetting.CameraType.main && gameProgress.worldIsActivated.Value;
            gameObject.SetActive(enabled);
        }
    }
}
