using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;
using System;
using System.Collections.Generic;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class PlayerPresenter : MonoBehaviour
    {
        private PlayerView view;
        private Player player;
        private PlaySetting playSetting;
        private Dictionary<Camera, PlaySetting.CameraType> cameraToTypes;
        private Dictionary<PlaySetting.CameraType, Camera> typeToCamera;

        void Start()
        {
            view = GetComponent<PlayerView>();
            player = Singleton<Player>.Instance;
            playSetting = Singleton<PlaySetting>.Instance;

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

            // Init
            player.currentWorld
                .Subscribe(_ => Init())
                .AddTo(gameObject);

            // SetPlayerPosition
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Subscribe(_ => UpdatePlayerInfo())
                .AddTo(gameObject);

            // ToggleCamera
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.F5))
                .ThrottleFirst(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ => ToggleCamera())
                .AddTo(gameObject);

            view.currentCamera
                .Subscribe(SetCurrentCameraToModel)
                .AddTo(gameObject);

            playSetting.cameraType
                .Subscribe(SetCurrentCameraToView)
                .AddTo(gameObject);

            // MoveGaze
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Subscribe(_ => MoveGaze())
                .AddTo(gameObject);

            // SetVelocityScale
            Observable
                .EveryFixedUpdate()
                .Where(_ => player.isOperable.Value)
                .Subscribe(_ => SetVelocityScale())
                .AddTo(gameObject);

            // MoveToForward
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.W))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToForward())
                .AddTo(gameObject);

            // MoveToBack
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.S))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToBack())
                .AddTo(gameObject);

            // MoveToLeft
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.A))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToLeft())
                .AddTo(gameObject);

            // MoveToRight
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.D))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToRight())
                .AddTo(gameObject);

            // Jump
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .Where(_ => Input.GetKey(KeyCode.Space))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => Jump())
                .AddTo(gameObject);
        }

        private void Init()
        {
            view.Init(player.position.Value);
        }

        private void UpdatePlayerInfo()
        {
            player.position.Value = view.transform.position;
            player.rotation.Value = view.transform.rotation;
            player.headRotation.Value = view.head.transform.rotation;
            player.gaze.Value = view.mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        private void ToggleCamera()
        {
            playSetting.ToggleCameraType();
        }

        private void SetCurrentCameraToModel(Camera currentCamera)
        {
            playSetting.cameraType.Value = cameraToTypes[currentCamera];
        }

        private void SetCurrentCameraToView(PlaySetting.CameraType cameraType)
        {
            view.currentCamera.Value = typeToCamera[cameraType];
        }

        private void MoveGaze()
        {
            var xRotation = playSetting.rotationSpeed.Value * Input.GetAxis("Mouse Y");
            var yRotation = playSetting.rotationSpeed.Value * Input.GetAxis("Mouse X");
            view.MoveGaze(xRotation, yRotation);
        }

        private void SetVelocityScale()
        {
            view.SetVelocityScale(player.CurrentBlock());
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
