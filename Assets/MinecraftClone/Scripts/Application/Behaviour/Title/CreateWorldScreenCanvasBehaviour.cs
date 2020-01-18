using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;
using UnityEngine.UI;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour.Title
{
    class CreateWorldScreenCanvasBehaviour : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField]
        public GameObject titleScreen = null;

        [Header("Forms")]
        public InputField worldSeedField = null;

        public void OnClickCancelButton()
        {
            Debug.Log("## Cancel");
            GetComponent<Canvas>().enabled = false;
            titleScreen.GetComponent<Canvas>().enabled = true;
        }

        public void OnClickCreateWorldButton()
        {
            Debug.Log("## Create world");
            if (worldSeedField.text != "")
            {
                Int32.TryParse(worldSeedField.text, out int baseSeed);
                Singleton<TerrainService>.Instance.Seed = new Domain.Seed(baseSeed);
            }
            else {
                Singleton<TerrainService>.Instance.Seed = new Domain.Seed();
            }
            SceneManager.LoadScene("World");
        }
    }
}