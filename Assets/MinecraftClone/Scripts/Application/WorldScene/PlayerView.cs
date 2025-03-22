using UnityEngine;
using UniRx;
using System;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Application.WorldScene
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
        private Rigidbody myRigidbody;

        void Awake()
        {
            cameras = new Camera[] { mainCamera, subCamera1, subCamera2 };
            myRigidbody = GetComponent<Rigidbody>();
            velocityScale = 1f;
            currentCamera = new ReactiveProperty<Camera>(mainCamera);
        }

        void Start()
        {
            currentCamera
                .Subscribe(ChangeCamera)
                .AddTo(gameObject);
        }

        public void Init(
            Vector3 position,
            Quaternion rotation,
            Quaternion headRotation,
            Camera camera
        )
        {
            transform.position = position;
            transform.rotation = rotation;
            head.transform.rotation = headRotation;
            currentCamera.Value = camera;
        }

        public void MoveGaze(float xRotation, float yRotation)
        {
            head.transform.Rotate(-xRotation, 0, 0);
            transform.Rotate(0, yRotation, 0);
        }

        public void SetVelocityScale(Block currentBlock)
        {
            if (currentBlock.Traits.MatterType == BlockTraits.MatterTypeEnum.Fluid)
            {
                myRigidbody.linearDamping = 3f;
                velocityScale = 0.5f;
            }
            else
            {
                myRigidbody.linearDamping = 0.01f;
                velocityScale = 1f;
            }
        }

        public void MoveToForward()
        {
            myRigidbody.linearVelocity = transform.TransformDirection(new Vector3(
                5 * velocityScale,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).y,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).z
            ));
        }

        public void MoveToBack()
        {
            myRigidbody.linearVelocity = transform.TransformDirection(new Vector3(
                -5 * velocityScale,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).y,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).z
            ));
        }

        public void MoveToLeft()
        {
            myRigidbody.linearVelocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(myRigidbody.linearVelocity).x,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).y,
                5 * velocityScale
            ));
        }

        public void MoveToRight()
        {
            myRigidbody.linearVelocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(myRigidbody.linearVelocity).x,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).y,
                -5 * velocityScale
            ));
        }

        public void Jump()
        {
            myRigidbody.linearVelocity = transform.TransformDirection(new Vector3(
                transform.InverseTransformDirection(myRigidbody.linearVelocity).x,
                5 * velocityScale,
                transform.InverseTransformDirection(myRigidbody.linearVelocity).z
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
