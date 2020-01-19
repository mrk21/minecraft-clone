using System;
using UniRx;

namespace MinecraftClone.Domain
{
    public class PlaySetting
    {
        public enum CameraType
        {
            main,
            sub1,
            sub2,
        };

        public static readonly CameraType[] CameraTypes = Enum.GetValues(typeof(CameraType)) as CameraType[];

        public ReactiveProperty<CameraType> cameraType;
        public ReactiveProperty<float> rotationSpeed;
        public ReactiveProperty<bool> isEnabledDebugScreen;

        public PlaySetting()
        {
            cameraType = new ReactiveProperty<CameraType>(CameraType.main);
            rotationSpeed = new ReactiveProperty<float>(3f);
            isEnabledDebugScreen = new ReactiveProperty<bool>(false);
        }

        public void ToggleCameraType()
        {
            var currentIndex = Array.FindIndex(CameraTypes, c => c == cameraType.Value);
            currentIndex = (currentIndex + 1) % CameraTypes.Length;
            cameraType.Value = CameraTypes[currentIndex];
        }
    }
}
