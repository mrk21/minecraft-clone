using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;
using UnityEngine.UI;
using MinecraftClone.Infrastructure;

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

            Debug.Log(Singleton<TerrainService>.Instance.Seed.Base);
            currentSeedField.text = Singleton<TerrainService>.Instance.Seed.Base.ToString();
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
