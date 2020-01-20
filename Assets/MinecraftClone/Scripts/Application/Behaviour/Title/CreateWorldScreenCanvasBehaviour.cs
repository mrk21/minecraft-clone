using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;
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
                Singleton<GameProgress>.Instance.MakeNewWorld(new Seed(baseSeed));
            }
            else
            {
                Singleton<GameProgress>.Instance.MakeNewWorld(new Seed());
            }
            SceneManager.LoadScene("World");
        }
    }
}