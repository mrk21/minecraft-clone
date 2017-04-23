using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		MapService mapService;
		public GameObject waterLevel = null; // set by Inspector
		public GameObject player = null; // set by Inspector

		void Start () {
			mapService = new MapService(gameObject, waterLevel);
			mapService.Init ();
			mapService.Draw (new Vector3 (0,0,0));
			setPlayer();
		}

		void Update () {
			if (Input.GetKey(KeyCode.R)) {
				mapService = new MapService(gameObject, waterLevel);
				setPlayer();
			}
			DrawWorld ();
		}

		void setPlayer() {
			player.transform.position = new Vector3 (30, 50, 30);
		}

		void DrawWorld() {
			mapService.Draw (player.transform.position);
		}
	}
}