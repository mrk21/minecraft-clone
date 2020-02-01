using System;
using UniRx;
using UnityEngine;

namespace MinecraftClone.Domain.Store
{
    public class PlaySetting
    {
        public enum CameraType
        {
            Main,
            Sub1,
            Sub2,
        };

        public enum InputType
        {
            Left,
            Right,
            Forword,
            Back,
            Jump,
            Put,
            Remove,
            ToggleCamera,
            ToggleDebugScreen,
        };

        public static CameraType[] CameraTypes { get; } = Enum.GetValues(typeof(CameraType)) as CameraType[];

        public ReactiveProperty<CameraType> Camera { get; } = new ReactiveProperty<CameraType>(CameraType.Main);
        public ReactiveProperty<float> RotationSpeed { get; } = new ReactiveProperty<float>(3f);
        public ReactiveProperty<bool> IsEnabledDebugScreen { get; } = new ReactiveProperty<bool>(false);

        public ReactiveDictionary<KeyCode, InputType> InputMap { get; } = new ReactiveDictionary<KeyCode, InputType>{
            { KeyCode.W, InputType.Forword },
            { KeyCode.D, InputType.Right },
            { KeyCode.A, InputType.Left },
            { KeyCode.S, InputType.Back },
            { KeyCode.Space, InputType.Jump },
            { KeyCode.Mouse1, InputType.Put },
            { KeyCode.Mouse0, InputType.Remove },
            { KeyCode.F5, InputType.ToggleCamera },
            { KeyCode.F2, InputType.ToggleDebugScreen },
        };

        public bool IsInput(KeyCode keyCode, InputType type)
        {
            return InputMap.ContainsKey(keyCode) && InputMap[keyCode] == type;
        }

        public void ToggleCameraType()
        {
            var currentIndex = Array.FindIndex(CameraTypes, c => c == Camera.Value);
            currentIndex = (currentIndex + 1) % CameraTypes.Length;
            Camera.Value = CameraTypes[currentIndex];
        }
    }
}
