using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class PlayerHeadBehaviour : MonoBehaviour {
		public MapService mapService;

		void Start() {
			if (mapService == null) mapService = Singleton<MapService>.Instance;
		}

		void Update () {
			if (Input.GetKey (KeyCode.UpArrow)) {
				transform.Rotate (new Vector3 (-4, 0, 0));
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				transform.Rotate (new Vector3 (4, 0, 0));
			}

			if (Input.GetMouseButtonDown (0)) {
				var chunk = mapService.CurrentChunk;
				var address = GetBlockAddress();

				if (address.HasValue) {
					if (chunk [address.Value + Vector3.down].Traits.IsBreakable()) {
						chunk [address.Value + Vector3.down].RemoveFromTerrain ();
						mapService.RedrawCurrentChunk ();
					}
					else if (chunk [address.Value + Vector3.down].Traits.IsReplaceable() && chunk [address.Value].Traits.IsBreakable()) {
						chunk [address.Value].RemoveFromTerrain ();
						mapService.RedrawCurrentChunk ();
					}
				}
			}

			if (Input.GetMouseButtonDown (1)) {
				var chunk = mapService.CurrentChunk;
				var address = GetBlockAddress();

				if (address.HasValue) {
					if (chunk [address.Value].Traits.IsReplaceable()) {
						chunk [address.Value] = new GrassBlock ();
						mapService.RedrawCurrentChunk ();
					}
				}
			}
		}

		Vector3? GetBlockAddress() {
			var distance = 100f;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();

			if (Physics.Raycast (ray, out hit, distance)) {
				return mapService.CurrentChunk.GetLocalPosition (hit.point);
			}
			return new Vector3?();
		}
	}
}