using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MinecraftClone.Domain.Store;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.TitleScene
{
    class MenuManagerPresenter : MonoBehaviour
    {
        [SerializeField] private MainScreenView view = null;

        private GameProgress gameProgress;
        private World world;

        void Start()
        {
            gameProgress = GameProgress.Get();
            world = gameProgress.CurrentWorld;

            var menuScene = SceneManager.GetSceneByName("Menu");
            SceneManager.SetActiveScene(menuScene);

            view.currentSeedField.text = world.currentDimension.Value.Seed.Base.ToString();

            view.closeMenuButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickCloseMenuButton())
                .AddTo(gameObject);

            view.backToTitleButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickBackToTitleButton())
                .AddTo(gameObject);
        }

        private void OnClickCloseMenuButton()
        {
            Scene menuScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(menuScene);
        }

        private void OnClickBackToTitleButton()
        {
            SceneManager.LoadScene("Title");
        }
    }
}