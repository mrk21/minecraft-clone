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
        private GameProgress gameProgress = null;
        private World world = null;
        private Player player = null;

        private void Awake()
        {
            gameProgress = GameProgress.Get();

            if (gameProgress.CurrentWorld == null)
            {
                world = gameProgress.MakeWorld(new Seed());
                gameProgress.JoinWorld(world.Id.Value);
            }
            world = gameProgress.CurrentWorld.Value;
            player = world.Player.Value;
        }

        void Start()
        {
            // GoToMenu
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.WorldIsActivated.Value)
                .Where(_ => Input.GetKeyDown(KeyCode.Backspace))
                .Subscribe(_ => GoToMenu())
                .AddTo(gameObject);

            // Deactivate
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.WorldIsActivated.Value)
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => Deactivate())
                .AddTo(gameObject);

            gameProgress.WorldIsActivated
                .Where(isActivated => !isActivated)
                .Subscribe(_ => Deactivate())
                .AddTo(gameObject);

            // Activate
            Observable
                .EveryUpdate()
                .Where(_ => gameProgress.WorldIsActivated.Value)
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => Activate())
                .AddTo(gameObject);

            player.IsOperable
                .Where(is_ => is_)
                .Subscribe(_ => Activate())
                .AddTo(gameObject);

            gameProgress.WorldIsActivated
                .Where(isActivated => isActivated)
                .Where(_ => player.CurrentDimension.HasValue)
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
            player.IsOperable.Value = false;
        }

        private void Activate()
        {
            Cursor.lockState = CursorLockMode.Locked;
            player.IsOperable.Value = true;
        }
    }
}