using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Map;

namespace MinecraftClone.Domain.Renderer {
	class MapRenderer {
		private Map.Map map;

		private GameObject chunkPrefab;
		private GameObject target;
		private GameObject waterLevel;

		public MapRenderer(Map.Map map, GameObject target, GameObject waterLevel, GameObject chunkPrefab) {
			this.map = map;
			this.target = target;
			this.waterLevel = waterLevel;
			this.chunkPrefab = chunkPrefab;
		}

		public void Init() {
			Unload ();

			var realSize = 100f;
			var center = realSize / 2f;
			var scale = realSize / 100f * 10f;

			waterLevel.transform.position = new Vector3 (center, Map.Map.WaterHeight, center);
			waterLevel.transform.localScale = new Vector3 (scale, 1, scale);
		}

		public void Unload() {
			foreach (var chunk in map.Chunks.Values) {
				GameObject.Destroy (chunk.GameObject);
				chunk.GameObject = null;
			}
		}

		public bool IsDrawed(Vector3 position) {
			return IsDrawed (ChunkAddress.FromPosition (position));
		}

		public bool IsDrawed(ChunkAddress address) {
			return map.IsGenerated (address) && map [address].GameObject != null;
		}

		public void Redraw(Vector3 position) {
			Redraw (ChunkAddress.FromPosition (position));
		}

		public void Redraw(ChunkAddress address) {
			var chunk = map [address];
			var factory = new ChunkMeshFactory (chunk);
			var mesh = factory.Create();
			chunk.GameObject.GetComponent<MeshFilter> ().mesh = mesh;
			chunk.GameObject.GetComponent<MeshCollider> ().sharedMesh = mesh;
		}

		public void Draw(Vector3 position) {
			Draw (ChunkAddress.FromPosition (position));
		}

		public void Draw(ChunkAddress address) {
			var chunk = map [address];
			if (!IsDrawed (address)) {
				var obj = GameObject.Instantiate(
					chunkPrefab,
					chunk.Address.ToPosition(),
					Quaternion.identity,
					target.transform
				);
				var factory = new ChunkMeshFactory (chunk);
				var mesh = factory.Create();
				obj.GetComponent<MeshFilter> ().mesh = mesh;
				obj.GetComponent<MeshCollider> ().sharedMesh = mesh;
				obj.SetActive (true);
				chunk.GameObject = obj;
			}
		}
	}
}