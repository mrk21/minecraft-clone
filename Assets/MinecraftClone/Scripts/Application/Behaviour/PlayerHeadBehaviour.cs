using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;
using UniRx;
using UnityEngine.UI;
using System;

namespace MinecraftClone.Application.Behaviour
{
    class PlayerHeadBehaviour : MonoBehaviour
    {
        [SerializeField] public Camera mainCamera = null;
        [SerializeField] public Camera subCamera1 = null;
        [SerializeField] public Camera subCamera2 = null;

        public Subject<Camera> OnCameraChanged = new Subject<Camera>();

        private long currentCameraIndex;
        private TerrainService terrainService;
        private Camera[] cameras;

        void Start()
        {
            if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
            currentCameraIndex = 0;
            mainCamera.enabled = true;
            subCamera1.enabled = false;
            subCamera2.enabled = false;
            cameras = new Camera[] { mainCamera, subCamera1, subCamera2 };

            // ChangeCamera
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetKey(KeyCode.F5))
                .ThrottleFirst(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ => ChangeCamera())
                .AddTo(gameObject);

            // PutBlock
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetMouseButtonDown(1))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => PutBlock())
                .AddTo(gameObject);

            // RemoveBlock
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Where(_ => Input.GetMouseButtonDown(0))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .BatchFrame(0, FrameCountType.FixedUpdate)
                .Subscribe(_ => RemoveBlock())
                .AddTo(gameObject);

            // ChangeView
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .Subscribe(_ => ChangeView())
                .AddTo(gameObject);
        }

        void ChangeCamera()
        {
            foreach (var camera in cameras)
            {
                camera.enabled = false;
            }

            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
            CurrentCamera.enabled = true;
            OnCameraChanged.OnNext(CurrentCamera);
        }

        public Camera CurrentCamera {
            get { return cameras[currentCameraIndex]; }
        }

        void PutBlock()
        {
            var hit = GetRaycastHit();

            if (hit.HasValue)
            {
                Vector3 position = hit.Value.point + 0.5f * hit.Value.normal;

                if (terrainService.Blocks[position].Traits.IsReplaceable())
                {
                    var block = new GrassBlock();
                    terrainService.Blocks[position] = block;
                    terrainService.RedrawChunk(position);
                }
            }
        }

        void ChangeView()
        {
            float xRotation = -3.0f * Input.GetAxis("Mouse Y");
            transform.Rotate(xRotation, 0, 0);
        }

        void RemoveBlock()
        {
            var hit = GetRaycastHit();

            if (hit.HasValue)
            {
                Vector3 position = hit.Value.point - 0.5f * hit.Value.normal;

                if (terrainService.Blocks[position].Traits.IsBreakable())
                {
                    terrainService.Blocks[position].RemoveFromTerrain();
                    terrainService.RedrawChunk(position);
                    var factory = new FluidPropagatorFactory();
                    StartCoroutine("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(terrainService.World, position));
                }
                else if (terrainService.Blocks[position].Traits.IsReplaceable() && terrainService.Blocks[position].Traits.IsBreakable())
                {
                    terrainService.Blocks[position].RemoveFromTerrain();
                    terrainService.RedrawChunk(position);
                    var factory = new FluidPropagatorFactory();
                    StartCoroutine("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(terrainService.World, position));
                }
            }
        }

        IEnumerator DrawFluid(FluidPropagator propagator)
        {
            yield return new WaitForSeconds(0.5f);

            foreach (var items in propagator.Start())
            {
                var chunkAddresses = new HashSet<ChunkAddress>();
                foreach (var item in items)
                {
                    terrainService.World.Blocks[item.Position] = item.Block;
                    chunkAddresses.Add(ChunkAddress.FromPosition(item.Position));
                }
                foreach (var address in chunkAddresses)
                {
                    terrainService.RedrawChunk(address);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        RaycastHit? GetRaycastHit()
        {
            var distance = 10f;
            Ray ray = GetRay();

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                return hit;
            }
            return new RaycastHit?();
        }

        Ray GetRay()
        {
            return mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}