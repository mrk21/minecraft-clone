﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class PlayerHeadBehaviour : MonoBehaviour {
		public TerrainService terrainService;

		private bool isPuttingBlock = false;
		private bool isRemovingBlock = false;

		void Start() {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
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
			var distance = 100f;
			Ray ray = GetRay();

			if (Physics.Raycast (ray, out RaycastHit hit, distance)) {
				return hit;
			}
			return new RaycastHit?();
		}

		Ray GetRay() {
			return Camera.main.ScreenPointToRay(Input.mousePosition);
		}

		bool EnabledOperation() {
			return Cursor.lockState == CursorLockMode.Locked;
		}
	}
}