using UnityEngine;
using UniRx;

namespace MinecraftClone.Application.TitleScene
{
    class TitleManagerPresenter : MonoBehaviour
    {
        public enum ScreenType
        {
            Top,
            CreateWorld
        }

        public ReactiveProperty<ScreenType> currentScreen;

        void Awake()
        {
            currentScreen = new ReactiveProperty<ScreenType>();
        }

        void Start()
        {
            currentScreen.Value = ScreenType.Top;
        }
    }
}