using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.TitleScene
{
    class CreateWorldScreenPresenter : MonoBehaviour
    {
        [SerializeField] private TitleManagerPresenter parent = null;

        private CreateWorldScreenView view;
        private GameProgress gameProgress;

        void Start()
        {
            view = GetComponent<CreateWorldScreenView>();
            gameProgress = Singleton<GameProgress>.Instance;

            parent.currentScreen
                .Subscribe(OnTransitionScreen)
                .AddTo(gameObject);

            view.cancelButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickCancelButton())
                .AddTo(gameObject);

            view.createWorldButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickCreateWorldButton())
                .AddTo(gameObject);
        }

        private void OnTransitionScreen(TitleManagerPresenter.ScreenType currentScreen)
        {
            view.SetEnabled(currentScreen == TitleManagerPresenter.ScreenType.CreateWorld);
        }

        private void OnClickCancelButton()
        {
            parent.currentScreen.Value = TitleManagerPresenter.ScreenType.Top;
        }

        private void OnClickCreateWorldButton()
        {
            if (view.worldSeedField.text != "")
            {
                Int32.TryParse(view.worldSeedField.text, out int baseSeed);
                gameProgress.MakeNewWorld(new Seed(baseSeed));
            }
            else
            {
                gameProgress.MakeNewWorld(new Seed());
            }
            SceneManager.LoadScene("World");
        }
    }
}