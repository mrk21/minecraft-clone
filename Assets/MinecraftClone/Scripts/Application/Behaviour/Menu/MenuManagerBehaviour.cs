using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.Behaviour.Menu
{
    class MenuManagerBehaviour : MonoBehaviour
    {
        [Header("Forms")]
        public InputField currentSeedField = null;

        void Start()
        {
            Debug.Log("## Menu Started");
            var menuScene = SceneManager.GetSceneByName("Menu");
            SceneManager.SetActiveScene(menuScene);

            var seed = Singleton<GameProgress>.Instance.currentWorld.Value.Seed;
            Debug.Log(seed.Base);
            currentSeedField.text = seed.Base.ToString();
        }

        public void OnClickBackToTitleButton()
        {
            Debug.Log("## Back to title");
            SceneManager.LoadScene("Title");
        }

        public void OnClickCloseMenuButton()
        {
            Debug.Log("## Close Menu");
            Scene menuScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(menuScene);
        }
    }
}
