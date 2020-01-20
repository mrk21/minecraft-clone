using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MinecraftClone.Domain;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    class WorldManagerPresenter : MonoBehaviour
    {
        private Player player = null;
        private GameProgress gameProgress = null;

        void Start()
        {
            player = Singleton<Player>.Instance;
            gameProgress = Singleton<GameProgress>.Instance;

            if (!gameProgress.currentWorld.HasValue || gameProgress.currentWorld.Value == null)
            {
                gameProgress.MakeNewWorld(new Seed());
            }

            // GoToMenu
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.worldIsActivated.Value)
                .Where(_ => Input.GetKeyDown(KeyCode.Backspace))
                .Subscribe(_ => GoToMenu())
                .AddTo(gameObject);

            // Deactivate
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.worldIsActivated.Value)
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => Deactivate())
                .AddTo(gameObject);

            gameProgress.worldIsActivated
                .Where(isActivated => !isActivated)
                .Subscribe(_ => Deactivate())
                .AddTo(gameObject);

            // Activate
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.worldIsActivated.Value)
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => Activate())
                .AddTo(gameObject);

            player.isOperable
                .Where(is_ => is_)
                .Subscribe(_ => Activate())
                .AddTo(gameObject);

            gameProgress.worldIsActivated
                .Where(isActivated => isActivated)
                .Where(_ => player.currentWorld.HasValue)
                .Subscribe(_ => Activate())
                .AddTo(gameObject);
        }

        void OnDestroy()
        {
            Deactivate();
        }

        private void GoToMenu()
        {
            Scene menuScene = SceneManager.GetSceneByName("Menu");
            if (!menuScene.isLoaded) SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        }

        private void Deactivate()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            player.isOperable.Value = false;
        }

        private void Activate()
        {
            Cursor.lockState = CursorLockMode.Locked;
            player.isOperable.Value = true;
        }
    }
}