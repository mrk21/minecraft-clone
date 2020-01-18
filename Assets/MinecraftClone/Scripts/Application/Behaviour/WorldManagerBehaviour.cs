using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;

namespace MinecraftClone.Application.Behaviour
{
    class WorldManagerBehaviour : MonoBehaviour
    {
        [SerializeField] private Canvas debugCanvas = null;
        [SerializeField] private Canvas cursorCanvas = null;
        [SerializeField] private PlayerBehaviour player = null;

        private bool enabledDebugCanvas = false;

        void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            debugCanvas.enabled = enabledDebugCanvas;

            // GoToMenu
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Backspace))
                .Where(_ => IsActive())
                .Subscribe(_ => GoToMenu())
                .AddTo(gameObject);

            // Toggle CursorCanvas
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.F2))
                .Where(_ => IsActive())
                .Subscribe(_ => ToggleDebugCanvas())
                .AddTo(gameObject);

            // Display CursorCanvas
            player.head.OnCameraChanged
                .Where(_ => IsActive())
                .Subscribe(_ => DisplayCursorCanvas())
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

        void GoToMenu()
        {
            Scene menuScene = SceneManager.GetSceneByName("Menu");
            if (!menuScene.isLoaded) SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        }

        void ToggleDebugCanvas()
        {
            enabledDebugCanvas = !enabledDebugCanvas;
            debugCanvas.enabled = enabledDebugCanvas;
        }

        private void DisplayCursorCanvas()
        {
            cursorCanvas.enabled = player.head.CurrentCamera == player.head.mainCamera;
        }

        void OnActiveSceneChanged(Scene prev, Scene next)
        {
            Debug.Log("## " + prev.name + "->" + next.name);

            if (prev.name == "World")
            {
                DisableOperation();
                debugCanvas.enabled = false;
                cursorCanvas.enabled = false;
            }
            else if (next.name == "World")
            {
                EnableOperation();
                debugCanvas.enabled = enabledDebugCanvas;
                DisplayCursorCanvas();
            }
        }

        private void OnDestroy()
        {
            DisableOperation();
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