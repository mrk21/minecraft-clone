using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		public GameObject waterLevel = null; // set by the inspector
		public GameObject player = null; // set by the inspector
		public MapService mapService;

		void Start () {
			if (mapService == null) mapService = Singleton<MapService>.Instance;
			mapService.Init(terrain: gameObject, waterLevel: waterLevel, player: player);
		}

		void Update () {
			if (Input.GetKey(KeyCode.R)) Start ();
			mapService.DrawAroundPlayer ();
		}
	}
}