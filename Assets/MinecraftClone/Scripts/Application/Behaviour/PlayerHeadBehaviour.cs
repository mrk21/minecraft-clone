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

			if (Input.GetMouseButtonDown(1)) {
				var distance = 100f;

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();

				if (Physics.Raycast(ray, out hit, distance)) {
					var gameObject = hit.collider.gameObject;

					if (gameObject.GetComponent<BlockBehavior> () != null) {
						var position = gameObject.transform.position;
						position += new Vector3 (0, 1, 0);
						mapService.PutBlock (new GrassBlock (), position);
					}
				}
			}

			if (Input.GetMouseButtonDown(0)) {
				var distance = 100f;

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();

				if (Physics.Raycast(ray, out hit, distance)) {
					var gameObject = hit.collider.gameObject;
					if (gameObject.GetComponent<BlockBehavior> () != null) {
						Destroy (gameObject);
					}
				}
			}
		}
	}
}