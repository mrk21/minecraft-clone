using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;

namespace MinecraftClone.Application.Behaviour.Title
{
    class TopScreenCanvasBehaviour : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField]
        public GameObject createWorldScreen = null;

        public void OnClickPlayButton()
        {
            Debug.Log("## Play");
            GetComponent<Canvas>().enabled = false;
            createWorldScreen.GetComponent<Canvas>().enabled = true;
        }

        public void OnClickQuitButton()
        {
            Debug.Log("## Quit");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
                UnityEngine.Application.Quit();
            #endif
        }
    }
}