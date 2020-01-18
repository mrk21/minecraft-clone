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
        [SerializeField] public PlayerHeadBehaviour head = null;

        private TerrainService terrainService;
        private float velocityScale = 1f;
        private Rigidbody _rigidbody;

        void Start()
        {
            if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
            _rigidbody = GetComponent<Rigidbody>();

            // SetVelocityScale
            Observable
                .EveryFixedUpdate()
                .Where(_ => EnabledOperation())
                .Subscribe(_ => SetVelocityScale())
                .AddTo(gameObject);

            // MoveToForward
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.W))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToForward())
                .AddTo(gameObject);

            // MoveToBack
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.S))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToBack())
                .AddTo(gameObject);

            // MoveToLeft
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.A))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToLeft())
                .AddTo(gameObject);

            // MoveToRight
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.D))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => MoveToRight())
                .AddTo(gameObject);

            // Jump
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.Space))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => Jump())
                .AddTo(gameObject);

            // ChangeView
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Subscribe(_ => ChangeView())
                .AddTo(gameObject);
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

        bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}