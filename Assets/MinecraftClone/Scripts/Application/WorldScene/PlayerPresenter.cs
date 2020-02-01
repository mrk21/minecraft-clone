using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Store;
using System.Linq;

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
            InputManager.Get().OnKey
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.ToggleCamera))
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
            InputManager.Get().OnKey
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Forword))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToForward())
                .AddTo(gameObject);

            // MoveToBack
            InputManager.Get().OnKey
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Back))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToBack())
                .AddTo(gameObject);

            // MoveToLeft
            InputManager.Get().OnKey
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Left))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToLeft())
                .AddTo(gameObject);

            // MoveToRight
            InputManager.Get().OnKey
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Right))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToRight())
                .AddTo(gameObject);

            // Jump
            InputManager.Get().OnKey
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Jump))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => Jump())
                .AddTo(gameObject);

            // PutBlock
            InputManager.Get().OnKeyDown
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Put))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => PutBlock())
                .AddTo(gameObject);

            // RemoveBlock
            InputManager.Get().OnKeyDown
                .Where(_ => player.IsOperable.Value)
                .Where(k => playSetting.IsInput(k, PlaySetting.InputType.Remove))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => RemoveBlock())
                .AddTo(gameObject);
        }

        private void Init()
        {
            cameraToTypes = new Dictionary<Camera, PlaySetting.CameraType>
            {
                { view.mainCamera, PlaySetting.CameraType.Main },
                { view.subCamera1, PlaySetting.CameraType.Sub1 },
                { view.subCamera2, PlaySetting.CameraType.Sub2 }
            };

            typeToCamera = new Dictionary<PlaySetting.CameraType, Camera>
            {
                { PlaySetting.CameraType.Main, view.mainCamera },
                { PlaySetting.CameraType.Sub1, view.subCamera1 },
                { PlaySetting.CameraType.Sub2, view.subCamera2 }
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
            player.OnMoveGaze.OnNext(Unit.Default);
        }

        private void SetVelocityScale()
        {
            view.SetVelocityScale(player.CurrentBlock.Value);
        }

        private void MoveToForward()
        {
            view.MoveToForward();
            player.OnMoveToForward.OnNext(Unit.Default);
        }

        private void MoveToBack()
        {
            view.MoveToBack();
            player.OnMoveToBack.OnNext(Unit.Default);
        }

        private void MoveToLeft()
        {
            view.MoveToLeft();
            player.OnMoveToLeft.OnNext(Unit.Default);
        }

        private void MoveToRight()
        {
            view.MoveToRight();
            player.OnMoveToRight.OnNext(Unit.Default);
        }

        private void Jump()
        {
            view.Jump();
            player.OnJump.OnNext(Unit.Default);
        }

        private void PutBlock()
        {
            player.OnPut.OnNext(Unit.Default);
        }

        private void RemoveBlock()
        {
            player.OnRemove.OnNext(Unit.Default);
        }
    }
}
