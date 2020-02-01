using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using System;
using System.Linq;

namespace MinecraftClone.Domain.Store
{
    public class InputManager
    {
        public static InputManager Get()
        {
            return Singleton<InputManager>.Instance;
        }

        public Subject<KeyCode> OnKey { get; } = new Subject<KeyCode>();
        public Subject<KeyCode> OnKeyDown { get; } = new Subject<KeyCode>();

        public InputManager()
        {
            var keyCodes = (Enum.GetValues(typeof(KeyCode)) as KeyCode[]).ToList();

            Observable
                .EveryUpdate()
                .Where(_ => Input.anyKey)
                .Select(_ => keyCodes.Find(Input.GetKey))
                .Subscribe(k => OnKey.OnNext(k));

            Observable
                .EveryUpdate()
                .Where(_ => Input.anyKeyDown)
                .Select(_ => keyCodes.Find(Input.GetKeyDown))
                .Subscribe(k => OnKeyDown.OnNext(k));
        }
    }
}
