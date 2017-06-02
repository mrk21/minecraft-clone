using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class PlayerHeadBehaviour : MonoBehaviour {
		public TerrainService terrainService;

		private bool isLookingUp = false;
		private bool isLookingDown = false;
		private bool isPuttingBlock = false;
		private bool isRemovingBlock = false;

		void Start() {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
		}

		void Update () {
			isLookingUp = Input.GetKey (KeyCode.UpArrow);
			isLookingDown = Input.GetKey (KeyCode.DownArrow);
			isPuttingBlock = Input.GetMouseButtonDown (0);
			isRemovingBlock = Input.GetMouseButtonDown (1);
		}

		void FixedUpdate () {
			if (isLookingUp) transform.Rotate (new Vector3 (-4, 0, 0));
			if (isLookingDown) transform.Rotate (new Vector3 (4, 0, 0));

			if (isPuttingBlock) {
				var chunk = terrainService.CurrentChunk;
				var address = GetBlockAddress();

				if (address.HasValue) {
					if (chunk [address.Value + Vector3.down].Traits.IsBreakable()) {
						chunk [address.Value + Vector3.down].RemoveFromTerrain ();
						terrainService.RedrawCurrentChunk ();
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(terrainService.CurrentChunk, address.Value + Vector3.down));
					}
					else if (chunk [address.Value + Vector3.down].Traits.IsReplaceable() && chunk [address.Value].Traits.IsBreakable()) {
						chunk [address.Value].RemoveFromTerrain ();
						terrainService.RedrawCurrentChunk ();
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromRemovingAdjoiningBlock (terrainService.CurrentChunk, address.Value));
					}
				}
			}

			if (isRemovingBlock) {
				var chunk = terrainService.CurrentChunk;
				var address = GetBlockAddress();

				if (address.HasValue) {
					if (chunk [address.Value].Traits.IsReplaceable()) {
						var block = new WaterBlock ();
						var position = address.Value;
						chunk [position] = block;
						terrainService.RedrawCurrentChunk ();
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromSource (terrainService.CurrentChunk, block, position));
					}
				}
			}
		}

		IEnumerator DrawFluid(FluidPropagator propagator) {
			yield return new WaitForSeconds (0.5f);

			foreach (var items in propagator.Start ()) {
				foreach (var item in items) {
					propagator.Chunk [item.Position] = item.Block;
				}
				terrainService.RedrawCurrentChunk ();
				yield return new WaitForSeconds (0.5f);
			}
		}

		Vector3? GetBlockAddress() {
			var distance = 100f;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();

			if (Physics.Raycast (ray, out hit, distance)) {
				return terrainService.CurrentChunk.GetLocalPosition (hit.point);
			}
			return new Vector3?();
		}
	}
}