using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;
using UniRx;
using UnityEngine.UI;

namespace MinecraftClone.Application.Behaviour {
	class PlayerHeadBehaviour : MonoBehaviour {
		public TerrainService terrainService;

        public Camera mainCamera;
		public Camera subCamera1;
		public Camera subCamera2;
		private long currentCameraIndex;
		
		private bool isPuttingBlock = false;
		private bool isRemovingBlock = false;
		
		void Start() {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
			currentCameraIndex = 0;
			mainCamera.enabled = true;
			subCamera1.enabled = false;
			subCamera2.enabled = false;
			
			var cameraChangeObservable = Observable
	            .EveryUpdate()
	            .Where(_ => EnabledOperation() && Input.GetKey(KeyCode.F5));

            Observable.Throttle(cameraChangeObservable, System.TimeSpan.FromSeconds(0.2f))
				.Do(_ => ChangeCamera())
				.Subscribe();
		}

		void ChangeCamera() {
			var cameras = new Camera[]{ mainCamera, subCamera1, subCamera2 };
            foreach (var camera in cameras)
            {
				camera.enabled = false;
            }

			currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
			cameras[currentCameraIndex].enabled = true;
		}

		void Update () {
            if (EnabledOperation()) {
				float xRotation = -3.0f * Input.GetAxis("Mouse Y");
				transform.Rotate(xRotation, 0, 0);
			}

            isRemovingBlock = EnabledOperation() && Input.GetMouseButtonDown (0);
			isPuttingBlock = EnabledOperation() && Input.GetMouseButtonDown (1);
		}

		void FixedUpdate () {
			if (isRemovingBlock) {
				var hit = GetRaycastHit();

				if (hit.HasValue) {
					Vector3 position = hit.Value.point - 0.5f * hit.Value.normal;

                    if (terrainService.Blocks [position].Traits.IsBreakable()) {
						terrainService.Blocks [position].RemoveFromTerrain ();
						terrainService.RedrawChunk(position);
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(terrainService.World, position));
					}
					else if (terrainService.Blocks [position].Traits.IsReplaceable() && terrainService.Blocks [position].Traits.IsBreakable()) {
						terrainService.Blocks [position].RemoveFromTerrain ();
						terrainService.RedrawChunk(position);
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromRemovingAdjoiningBlock (terrainService.World, position));
					}
				}
			}

			if (isPuttingBlock) {
				var hit = GetRaycastHit();

				if (hit.HasValue) {
					Vector3 position = hit.Value.point + 0.5f * hit.Value.normal;

					if (terrainService.Blocks [position].Traits.IsReplaceable()) {
						var block = new GrassBlock ();
						terrainService.Blocks [position] = block;
						terrainService.RedrawChunk(position);
					}
				}
			}
		}

		IEnumerator DrawFluid(FluidPropagator propagator) {
			yield return new WaitForSeconds (0.5f);

			foreach (var items in propagator.Start ()) {
				var chunkAddresses = new HashSet<ChunkAddress> ();
				foreach (var item in items) {
					terrainService.World.Blocks [item.Position] = item.Block;
					chunkAddresses.Add (ChunkAddress.FromPosition (item.Position));
				}
				foreach (var address in chunkAddresses) {
					terrainService.RedrawChunk (address);
				}
				yield return new WaitForSeconds (0.5f);
			}
		}

		RaycastHit? GetRaycastHit() {
			var distance = 10f;
			Ray ray = GetRay();

			if (Physics.Raycast (ray, out RaycastHit hit, distance)) {
                return hit;
			}
			return new RaycastHit?();
		}

		Ray GetRay() {
			return mainCamera.ScreenPointToRay(Input.mousePosition);
		}

		bool EnabledOperation() {
			return Cursor.lockState == CursorLockMode.Locked;
		}
	}
}