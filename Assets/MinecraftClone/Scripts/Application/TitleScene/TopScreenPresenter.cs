using UnityEngine;
using UniRx;

namespace MinecraftClone.Application.TitleScene
{
    class TopScreenPresenter : MonoBehaviour
    {
        [SerializeField] private TitleManagerPresenter parent = null;

        private TopScreenView view;

        void Start()
        {
            view = GetComponent<TopScreenView>();

            parent.currentScreen
                .Subscribe(OnTransitionScreen)
                .AddTo(gameObject);

            view.playButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickPlayButton())
                .AddTo(gameObject);

            view.quitButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickQuitButton())
                .AddTo(gameObject);
        }

        private void OnTransitionScreen(TitleManagerPresenter.ScreenType currentScreen)
        {
            view.SetEnabled(currentScreen == TitleManagerPresenter.ScreenType.Top);
        }

        private void OnClickPlayButton()
        {
            parent.currentScreen.Value = TitleManagerPresenter.ScreenType.CreateWorld;
        }

        private void OnClickQuitButton()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
                UnityEngine.Application.Quit();
            #endif
        }
    }
}