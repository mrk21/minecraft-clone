using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain;

namespace MinecraftClone.Application.Behaviour {
	class TerrainBehaviour : MonoBehaviour {
		public MapFactory map;
		public GameObject waterLevel;
		public GameObject player;

		void Start () {
			map = new MapFactory (size: 2000, resolution: 20, minHeight: 0, maxHeight: 50, waterHeight: 30);
			map.Generate ();
			map.Draw (gameObject);
			player = GameObject.Find ("Player");
			waterLevel = GameObject.Find ("WaterLevel");
			setPlayer();
			setWaterLevel ();
		}

		void Update () {
			if (Input.GetKeyDown ("r")) {
				map.Generate ();
				map.Draw (gameObject);
				setPlayer();
			}
		}

		private void setPlayer() {
			player.transform.position = new Vector3 (30, map.maxHeight, 30);
		}

		private void setWaterLevel() {
			var size = 1f * map.size / map.resolution;
			var center = size / 2;
			var scale = size / 100;

			waterLevel.transform.position = new Vector3 (center, map.waterHeight, center);
			waterLevel.transform.localScale = new Vector3 (scale, 1, scale);
		}
	}
}
