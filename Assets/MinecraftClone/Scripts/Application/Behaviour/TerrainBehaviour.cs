using System.Collections;
using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		public GameObject player = null; // set by the inspector
		public MapService mapService;

		void Start () {
			if (mapService == null) mapService = Singleton<MapService>.Instance;
			mapService.Init(
				terrain: gameObject,
				player: player
			);
		}

		void Update () {
			if (Input.GetKey(KeyCode.R)) Start ();
			if (Input.GetKey (KeyCode.P)) mapService.Redraw ();
			mapService.DrawAroundPlayer ();
		}
	}
}