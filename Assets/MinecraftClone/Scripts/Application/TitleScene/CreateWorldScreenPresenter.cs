using System;
using System.Linq;
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

            view.worldList.onClickJoinButton
                .Subscribe(OnClickJoinWorldButton);

            view.worldList.onClickDeleteButton
                .Subscribe(OnClickDeleteWorldButton);

            gameProgress.worldList
                .ObserveAdd()
                .Subscribe(item => AddItem(item.Value))
                .AddTo(gameObject);

            gameProgress.worldList
                .ObserveRemove()
                .Subscribe(item => view.worldList.DeleteItem(item.Key))
                .AddTo(gameObject);

            Render();
        }

        private void Render()
        {
            gameProgress.worldList.Values.ToList().ForEach(AddItem);
        }

        private void OnTransitionScreen(TitleManagerPresenter.ScreenType currentScreen)
        {
            view.SetEnabled(currentScreen == TitleManagerPresenter.ScreenType.CreateWorld);
        }

        private void OnClickCancelButton()
        {
            parent.currentScreen.Value = TitleManagerPresenter.ScreenType.Top;
        }

        private void OnClickJoinWorldButton(WorldListItemView item)
        {
            gameProgress.JoinWorld(item.worldId);
            SceneManager.LoadScene("World");
        }

        private void OnClickDeleteWorldButton(WorldListItemView item)
        {
            gameProgress.worldList.Remove(item.worldId);
        }

        private void OnClickCreateWorldButton()
        {
            if (view.worldSeedField.text != "")
            {
                Int32.TryParse(view.worldSeedField.text, out int baseSeed);
                var seed = new Seed(baseSeed);
                gameProgress.MakeWorld(seed: seed);
            }
            else
            {
                var seed = new Seed();
                gameProgress.MakeWorld(seed: seed);
            }
        }

        private void AddItem(World world)
        {
            view.worldList.AddItem(
                id: world.Id,
                name: world.name.Value,
                seed: world.seed.Value
            );
        }
    }
}