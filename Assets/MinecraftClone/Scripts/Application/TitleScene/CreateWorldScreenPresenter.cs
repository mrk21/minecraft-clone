using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.TitleScene
{
    class CreateWorldScreenPresenter : MonoBehaviour
    {
        [SerializeField] private TitleManagerPresenter parent = null;

        private CreateWorldScreenView view;
        private GameProgress gameProgress;

        void Awake()
        {
            view = GetComponent<CreateWorldScreenView>();
            gameProgress = GameProgress.Get();
        }

        void Start()
        {
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
            gameProgress.WorldList
                .ObserveAdd()
                .Subscribe(item => AddItem(item.Value))
                .AddTo(gameObject);

            gameProgress.WorldList
                .ObserveRemove()
                .Subscribe(item => DeleteItem(item.Value))
                .AddTo(gameObject);

            OnCloseEditModal();
            OnCloseCreateModal();
            Render();
        }

        private void Render()
        {
            gameProgress.WorldList.Values.ToList().ForEach(AddItem);
        }

        private void OnTransitionScreen(TitleManagerPresenter.ScreenType currentScreen)
        {
            var enabled = currentScreen == TitleManagerPresenter.ScreenType.CreateWorld;
            if (!enabled) view.createWorldModal.SetEnabled(false);
            view.SetEnabled(enabled);
        }

        // back
        private void OnClickBackButton()
        {
            parent.currentScreen.Value = TitleManagerPresenter.ScreenType.Top;
        }

        // new
        private void OnClickNewWorldButton()
        {
            OnCloseEditModal();
            view.createWorldModal.Init(worldName: "New World", worldSeed: "");
            view.createWorldModal.SetEnabled(true);
        }

        private void OnCloseCreateModal()
        {
            view.createWorldModal.SetEnabled(false);
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

        private void AddItem(World world)
        {
            view.worldList.AddItem(item =>
            {
                world.Id
                    .Subscribe(value => item.worldId = value)
                    .AddTo(item.gameObject);

                world.Name
                    .Subscribe(value => item.worldName.text = value)
                    .AddTo(item.gameObject);

                world.Seed.Value.Base
                    .Subscribe(value => item.worldSeed.text = value.ToString())
                    .AddTo(item.gameObject);

                item.joinButton
                    .OnClickAsObservable()
                    .Subscribe(_ => OnClickJoinWorldButton(item))
                    .AddTo(item.gameObject);

                item.editButton
                    .OnClickAsObservable()
                    .Subscribe(_ => OnClickEditWorldButton(item))
                    .AddTo(item.gameObject);

                item.deleteButton
                    .OnClickAsObservable()
                    .Subscribe(_ => OnClickDeleteWorldButton(item))
                    .AddTo(item.gameObject);
            });
        }

        // join
        private void OnClickJoinWorldButton(WorldListItemView item)
        {
            gameProgress.JoinWorld(item.worldId);
            SceneManager.LoadScene("World");
        }

        // edit
        private void OnClickEditWorldButton(WorldListItemView item)
        {
            OnCloseCreateModal();
            var world = gameProgress.WorldList[item.worldId];
            view.editWorldModal.Init(
                worldId: world.Id.Value,
                worldName: world.Name.Value,
                worldSeed: world.Seed.Value.Base.ToString()
            );
            view.editWorldModal.SetEnabled(true);
        }

        private void OnCloseEditModal()
        {
            view.editWorldModal.SetEnabled(false);
        }

        private void OnUpdateWorld()
        {
            var modal = view.editWorldModal;
            var world = gameProgress.WorldList[modal.worldId];
            world.Name.Value = modal.worldNameField.text;
            view.editWorldModal.SetEnabled(false);
        }

        // delete
        private void OnClickDeleteWorldButton(WorldListItemView item)
        {
            gameProgress.WorldList.Remove(item.worldId);
        }

        private void DeleteItem(World world)
        {
            view.worldList.DeleteItem(world.Id.Value);
        }
    }
}