using System;
using UniRx;

namespace MinecraftClone.Domain.Store
{
    public class PlaySetting
    {
        public enum CameraType
        {
            main,
            sub1,
            sub2,
        };

        public static CameraType[] CameraTypes { get; } = Enum.GetValues(typeof(CameraType)) as CameraType[];

        public ReactiveProperty<CameraType> Camera { get; } = new ReactiveProperty<CameraType>(CameraType.main);
        public ReactiveProperty<float> RotationSpeed { get; } = new ReactiveProperty<float>(3f);
        public ReactiveProperty<bool> IsEnabledDebugScreen { get; } = new ReactiveProperty<bool>(false);

        public void ToggleCameraType()
        {
            var currentIndex = Array.FindIndex(CameraTypes, c => c == Camera.Value);
            currentIndex = (currentIndex + 1) % CameraTypes.Length;
            Camera.Value = CameraTypes[currentIndex];
        }
    }
}
