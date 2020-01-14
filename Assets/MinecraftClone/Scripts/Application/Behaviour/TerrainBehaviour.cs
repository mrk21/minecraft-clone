using System.Collections;
using UnityEngine;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		public GameObject player = null; // set by the inspector
		public GameObject debugScreen = null; // set by the inspector
		public TerrainService terrainService;

		void Start () {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
			terrainService.Init(
				terrain: gameObject,
				player: player
			);
			terrainService.DrawAroundPlayer ();
		}

		void Update () {
			debugScreen.GetComponent<DebugScreenBehaviour>().currentSeed = terrainService.World.Seed;
			debugScreen.GetComponent<DebugScreenBehaviour>().currentChunk = terrainService.CurrentChunk;
			debugScreen.GetComponent<DebugScreenBehaviour>().currentBlock = terrainService.CurrentBlock;
			debugScreen.GetComponent<DebugScreenBehaviour>().currentPosition = terrainService.CurrentPosition;

			if (Input.GetKey(KeyCode.R)) Start ();
			if (Input.GetKey (KeyCode.P)) terrainService.Redraw ();

			for (int x = -1; x <= 1; x++) {
				for (int z = -1; z <= 1; z++) {
					terrainService.DrawAroundPlayer (x, z);
				}
			}
		}
	}
}