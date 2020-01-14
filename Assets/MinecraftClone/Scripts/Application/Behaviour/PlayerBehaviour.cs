using UnityEngine;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;
using System;
using UniRx;
using UnityEngine.UI;

namespace MinecraftClone.Application.Behaviour
{
    class PlayerBehaviour : MonoBehaviour
    {
        public TerrainService terrainService;
        public PlayerHeadBehaviour head;

        private float velocityScale = 1f;
        private Rigidbody _rigidbody;

        void Start()
        {
            if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
            _rigidbody = GetComponent<Rigidbody>();
            EnableOperation();

            var settingVelocityScaleStream = Observable
                .EveryFixedUpdate()
                .Where(_ => EnabledOperation())
                .Subscribe(_ => SetVelocityScale());

            var movingToForwardStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.W))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToForward());

            var movingToBackStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.S))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToBack());

            var movingToLeftStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.A))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToLeft());

            var movingToRightStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.D))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToRight());

            var jumpingStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.Space))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => Jump());

            var changingViewStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Subscribe(_ => ChangeView());

            var disablingOperationStream = Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => DisableOperation());

            var enablingOperationStream = Observable
                .EveryUpdate()
                .Where(_ => !EnabledOperation())
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => EnableOperation());

        }

        void SetVelocityScale()
        {
            if (terrainService.BlockUnderPlayer() is FluidBlock || terrainService.BlockUnderPlayer(Vector3.up) is FluidBlock)
            {
                _rigidbody.drag = 3f;
                velocityScale = 0.5f;
            }
            else
            {
                _rigidbody.drag = 0.01f;
                velocityScale = 1f;
            }
        }

        void MoveToForward()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                5 * velocityScale,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                transform.InverseTransformDirection(_rigidbody.velocity).z
            ));
        }

        void MoveToBack()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                -5 * velocityScale,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                transform.InverseTransformDirection(_rigidbody.velocity).z
            ));
        }

        void MoveToLeft()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(_rigidbody.velocity).x,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                5 * velocityScale
            ));
        }

        void MoveToRight()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(_rigidbody.velocity).x,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                -5 * velocityScale
            ));
        }

        void Jump()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(_rigidbody.velocity).x,
                5 * velocityScale,
                transform.InverseTransformDirection(_rigidbody.velocity).z
            ));
        }

        void ChangeView()
        {
            float yRotation = 4.0f * Input.GetAxis("Mouse X");
            transform.Rotate(0, yRotation, 0);
        }

        void DisableOperation()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void EnableOperation()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}