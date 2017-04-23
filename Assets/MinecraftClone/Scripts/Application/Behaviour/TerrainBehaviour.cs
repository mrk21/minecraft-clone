using UnityEngine;
using MinecraftClone.Domain.Map;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		Map map;
		public GameObject waterLevel = null; // set by Inspector
		public GameObject player = null; // set by Inspector

		void Start () {
			Init ();
		}

		void Update () {
			if (Input.GetKey(KeyCode.R)) Init ();
			DrawWorld ();
		}

		void Init() {
			map = new Map(gameObject, waterLevel);
			map.Init ();
			player.transform.position = new Vector3 (60, 50, 60);
		}

		void DrawWorld() {
			map.Draw (player.transform.position);
		}
	}
}