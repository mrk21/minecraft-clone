using System.Collections;
using UnityEngine;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		public GameObject player = null; // set by the inspector
		public TerrainService terrainService;

		void Start () {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
			terrainService.Init(
				terrain: gameObject,
				player: player
			);
		}

		void Update () {
			if (Input.GetKey(KeyCode.R)) Start ();
			if (Input.GetKey (KeyCode.P)) terrainService.Redraw ();
			terrainService.DrawAroundPlayer ();
		}
	}
}