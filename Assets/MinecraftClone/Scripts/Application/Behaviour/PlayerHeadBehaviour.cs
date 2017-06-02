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
			isRemovingBlock = Input.GetMouseButtonDown (0);
			isPuttingBlock = Input.GetMouseButtonDown (1);
		}

		void FixedUpdate () {
			if (isLookingUp) transform.Rotate (new Vector3 (-4, 0, 0));
			if (isLookingDown) transform.Rotate (new Vector3 (4, 0, 0));

			if (isRemovingBlock) {
				var address = GetBlockAddress();

				if (address.HasValue) {
					if (terrainService.Blocks [address.Value + Vector3.down].Traits.IsBreakable()) {
						terrainService.Blocks [address.Value + Vector3.down].RemoveFromTerrain ();
						terrainService.RedrawCurrentChunk ();
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(terrainService.World, address.Value + Vector3.down));
					}
					else if (terrainService.Blocks [address.Value + Vector3.down].Traits.IsReplaceable() && terrainService.Blocks [address.Value].Traits.IsBreakable()) {
						terrainService.Blocks [address.Value].RemoveFromTerrain ();
						terrainService.RedrawCurrentChunk ();
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromRemovingAdjoiningBlock (terrainService.World, address.Value));
					}
				}
			}

			if (isPuttingBlock) {
				var address = GetBlockAddress();

				if (address.HasValue) {
					if (terrainService.Blocks [address.Value].Traits.IsReplaceable()) {
						var block = new WaterBlock ();
						var position = address.Value;
						terrainService.Blocks [position] = block;
						terrainService.RedrawCurrentChunk ();
						var factory = new FluidPropagatorFactory ();
						StartCoroutine ("DrawFluid", factory.CreateFromSource (terrainService.World, block, position));
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

		Vector3? GetBlockAddress() {
			var distance = 100f;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();

			if (Physics.Raycast (ray, out hit, distance)) {
				return hit.point;
			}
			return new Vector3?();
		}
	}
}