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

            view.backButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickBackButton())
                .AddTo(gameObject);

            view.newWorldButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickNewWorldButton())
                .AddTo(gameObject);

            view.worldList.onClickJoinButton
                .Subscribe(OnClickJoinWorldButton);

            view.worldList.onClickEditButton
                .Subscribe(OnClickEditWorldButton);

            view.worldList.onClickDeleteButton
                .Subscribe(OnClickDeleteWorldButton);

            // edit world modal
            view.editWorldModal.saveButton
                .OnClickAsObservable()
                .Subscribe(_ => OnUpdateWorld())
                .AddTo(gameObject);

            view.editWorldModal.cancelButton
                .OnClickAsObservable()
                .Subscribe(_ => OnCloseEditModal())
                .AddTo(gameObject);

            // create world modal
            view.createWorldModal.createButton
                .OnClickAsObservable()
                .Subscribe(_ => OnCreateWorld())
                .AddTo(gameObject);

            view.createWorldModal.cancelButton
                .OnClickAsObservable()
                .Subscribe(_ => OnCloseCreateModal())
                .AddTo(gameObject);

            // world list
            gameProgress.worldList
                .ObserveAdd()
                .Subscribe(item => AddItem(item.Value))
                .AddTo(gameObject);

            gameProgress.worldList
                .ObserveRemove()
                .Subscribe(item => view.worldList.DeleteItem(item.Key))
                .AddTo(gameObject);

            OnCloseEditModal();
            OnCloseCreateModal();
            Render();
        }

        private void Render()
        {
            gameProgress.worldList.Values.ToList().ForEach(AddItem);
        }

        private void OnUpdateWorld()
        {
            var modal = view.editWorldModal;
            var world = gameProgress.worldList[modal.worldId];
            world.name.Value = modal.worldNameField.text;
            view.worldList.items[world.Id].worldName.text = world.name.Value;
            view.editWorldModal.SetEnabled(false);
        }

        private void OnCreateWorld()
        {
            if (view.createWorldModal.worldSeedField.text != "")
            {
                Int32.TryParse(view.createWorldModal.worldSeedField.text, out int baseSeed);
                var name = view.createWorldModal.worldNameField.text;
                var seed = new Seed(baseSeed);
                gameProgress.MakeWorld(seed: seed, name: name);
            }
            else
            {
                var name = view.createWorldModal.worldNameField.text;
                var seed = new Seed();
                gameProgress.MakeWorld(seed: seed, name: name);
            }

            OnCloseCreateModal();
        }

        private void OnCloseEditModal()
        {
            view.editWorldModal.SetEnabled(false);
        }

        private void OnCloseCreateModal()
        {
            view.createWorldModal.SetEnabled(false);
        }

        private void OnTransitionScreen(TitleManagerPresenter.ScreenType currentScreen)
        {
            var enabled = currentScreen == TitleManagerPresenter.ScreenType.CreateWorld;
            if (!enabled) view.createWorldModal.SetEnabled(false);
            view.SetEnabled(enabled);
        }

        private void OnClickBackButton()
        {
            parent.currentScreen.Value = TitleManagerPresenter.ScreenType.Top;
        }

        private void OnClickJoinWorldButton(WorldListItemView item)
        {
            gameProgress.JoinWorld(item.worldId);
            SceneManager.LoadScene("World");
        }

        private void OnClickEditWorldButton(WorldListItemView item)
        {
            OnCloseCreateModal();
            var world = gameProgress.worldList[item.worldId];
            view.editWorldModal.Init(
                worldId: world.Id,
                worldName: world.name.Value,
                worldSeed: world.seed.Value.Base.ToString()
            );
            view.editWorldModal.SetEnabled(true);
        }

        private void OnClickDeleteWorldButton(WorldListItemView item)
        {
            gameProgress.worldList.Remove(item.worldId);
        }

        private void OnClickNewWorldButton()
        {
            OnCloseEditModal();
            view.createWorldModal.Init(worldName: "New World", worldSeed: "");
            view.createWorldModal.SetEnabled(true);
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