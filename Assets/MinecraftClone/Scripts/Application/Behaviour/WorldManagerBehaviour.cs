using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;

namespace MinecraftClone.Application.Behaviour
{
    class WorldManagerBehaviour : MonoBehaviour
    {
        void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            // GoToMenu
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Backspace))
                .Where(_ => SceneManager.GetActiveScene().name == "World")
                .Subscribe(_ => GoToMenu())
                .AddTo(gameObject);

            // DisableOperation
            Observable
                .EveryUpdate()
                .Where(_ => IsEnabledOperation())
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Where(_ => IsActive())
                .Subscribe(_ => DisableOperation())
                .AddTo(gameObject);

            // EnableOperation
            Observable
                .EveryUpdate()
                .Where(_ => !IsEnabledOperation())
                .Where(_ => Input.GetMouseButtonDown(0))
                .Where(_ => IsActive())
                .Subscribe(_ => EnableOperation())
                .AddTo(gameObject);
        }

        void OnActiveSceneChanged(Scene prev, Scene next)
        {
            Debug.Log("## " + prev.name + "->" + next.name);

            if (prev.name == "World")
            {
                DisableOperation();
            }
            else if (next.name == "World")
            {
                EnableOperation();
            }
        }

        private void OnDestroy()
        {
            DisableOperation();
        }

        void GoToMenu()
        {
            Scene menuScene = SceneManager.GetSceneByName("Menu");
            if (!menuScene.isLoaded) SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        }

        public static bool IsActive()
        {
            return SceneManager.GetActiveScene().name == "World";
        }

        public static void DisableOperation()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void EnableOperation()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static bool IsEnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}