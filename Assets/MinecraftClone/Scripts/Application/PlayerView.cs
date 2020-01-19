using UnityEngine;
using UniRx;
using System;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Application
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] public GameObject head = null;
        [SerializeField] public Camera mainCamera = null;
        [SerializeField] public Camera subCamera1 = null;
        [SerializeField] public Camera subCamera2 = null;

        public ReactiveProperty<Camera> currentCamera;

        private Camera[] cameras;
        private float velocityScale;
        private Rigidbody _rigidbody;

        void Start()
        {
            cameras = new Camera[] { mainCamera, subCamera1, subCamera2 };
            _rigidbody = GetComponent<Rigidbody>();
            velocityScale = 1f;

            currentCamera = new ReactiveProperty<Camera>(mainCamera);
            currentCamera
                .Subscribe(ChangeCamera)
                .AddTo(gameObject);
        }

        public void Init(Vector3 position)
        {
            transform.position = position;
        }

        public void MoveGaze(float xRotation, float yRotation)
        {
            head.transform.Rotate(-xRotation, 0, 0);
            transform.Rotate(0, yRotation, 0);
        }

        public void SetVelocityScale(BaseBlock currentBlock)
        {
            if (currentBlock is FluidBlock)
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

        public void MoveToForward()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                5 * velocityScale,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                transform.InverseTransformDirection(_rigidbody.velocity).z
            ));
        }

        public void MoveToBack()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                -5 * velocityScale,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                transform.InverseTransformDirection(_rigidbody.velocity).z
            ));
        }

        public void MoveToLeft()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(_rigidbody.velocity).x,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                5 * velocityScale
            ));
        }

        public void MoveToRight()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(_rigidbody.velocity).x,
                transform.InverseTransformDirection(_rigidbody.velocity).y,
                -5 * velocityScale
            ));
        }

        public void Jump()
        {
            _rigidbody.velocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(_rigidbody.velocity).x,
                5 * velocityScale,
                transform.InverseTransformDirection(_rigidbody.velocity).z
            ));
        }

        private void ChangeCamera(Camera camera)
        {
            if (Array.FindIndex(cameras, c => c == camera) == -1) throw new System.Exception("Invalid camera!");
            foreach (var c in cameras) c.enabled = false;
            camera.enabled = true;
        }
    }
}
