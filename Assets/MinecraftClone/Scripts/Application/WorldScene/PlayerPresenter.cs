using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class PlayerPresenter : MonoBehaviour
    {
        private PlayerView view;

        private GameProgress gameProgress;
        private Player player;
        private PlaySetting playSetting;

        private Dictionary<Camera, PlaySetting.CameraType> cameraToTypes;
        private Dictionary<PlaySetting.CameraType, Camera> typeToCamera;

        void Awake()
        {
            view = GetComponent<PlayerView>();
            gameProgress = GameProgress.Get();
            player = gameProgress.CurrentWorld.Value.Player.Value;
            playSetting = gameProgress.CurrentWorld.Value.PlaySetting.Value;
        }

        void Start()
        {
            // Init
            player.CurrentDimension
                .Subscribe(_ => Init())
                .AddTo(gameObject);

            // SetPlayerPosition
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Subscribe(_ => UpdatePlayerInfo())
                .AddTo(gameObject);

            // ToggleCamera
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.F5))
                .ThrottleFirst(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ => ToggleCamera())
                .AddTo(gameObject);

            view.currentCamera
                .Subscribe(SetCurrentCameraToModel)
                .AddTo(gameObject);

            playSetting.Camera
                .Subscribe(SetCurrentCameraToView)
                .AddTo(gameObject);

            // MoveGaze
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Subscribe(_ => MoveGaze())
                .AddTo(gameObject);

            // SetVelocityScale
            Observable
                .EveryFixedUpdate()
                .Where(_ => player.IsOperable.Value)
                .Subscribe(_ => SetVelocityScale())
                .AddTo(gameObject);

            // MoveToForward
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.W))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToForward())
                .AddTo(gameObject);

            // MoveToBack
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.S))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToBack())
                .AddTo(gameObject);

            // MoveToLeft
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.A))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToLeft())
                .AddTo(gameObject);

            // MoveToRight
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.D))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToRight())
                .AddTo(gameObject);

            // Jump
            Observable
                .EveryUpdate()
                .Where(_ => player.IsOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.Space))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => Jump())
                .AddTo(gameObject);
        }

        private void Init()
        {
            cameraToTypes = new Dictionary<Camera, PlaySetting.CameraType>
            {
                { view.mainCamera, PlaySetting.CameraType.main },
                { view.subCamera1, PlaySetting.CameraType.sub1 },
                { view.subCamera2, PlaySetting.CameraType.sub2 }
            };

            typeToCamera = new Dictionary<PlaySetting.CameraType, Camera>
            {
                { PlaySetting.CameraType.main, view.mainCamera },
                { PlaySetting.CameraType.sub1, view.subCamera1 },
                { PlaySetting.CameraType.sub2, view.subCamera2 }
            };

            view.Init(
                position: player.Position.Value,
                rotation: player.Rotation.Value,
                headRotation: player.HeadRotation.Value,
                camera: typeToCamera[playSetting.Camera.Value]
            );
        }

        private void UpdatePlayerInfo()
        {
            player.Position.Value = view.transform.position;
            player.Rotation.Value = view.transform.rotation;
            player.HeadRotation.Value = view.head.transform.rotation;
            player.Gaze.Value = view.mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        private void ToggleCamera()
        {
            playSetting.ToggleCameraType();
        }

        private void SetCurrentCameraToModel(Camera currentCamera)
        {
            playSetting.Camera.Value = cameraToTypes[currentCamera];
        }

        private void SetCurrentCameraToView(PlaySetting.CameraType cameraType)
        {
            view.currentCamera.Value = typeToCamera[cameraType];
        }

        private void MoveGaze()
        {
            var xRotation = playSetting.RotationSpeed.Value * Input.GetAxis("Mouse Y");
            var yRotation = playSetting.RotationSpeed.Value * Input.GetAxis("Mouse X");
            view.MoveGaze(xRotation, yRotation);
        }

        private void SetVelocityScale()
        {
            view.SetVelocityScale(player.CurrentBlock.Value);
        }

        private void MoveToForward()
        {
            view.MoveToForward();
        }

        private void MoveToBack()
        {
            view.MoveToBack();
        }

        private void MoveToLeft()
        {
            view.MoveToLeft();
        }

        private void MoveToRight()
        {
            view.MoveToRight();
        }

        private void Jump()
        {
            view.Jump();
        }
    }
}
