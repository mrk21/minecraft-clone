﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Terrain;

namespace MinecraftClone.Domain.Renderer {
	class TerrainRenderer {
		private World world;

		private GameObject chunkPrefab;
		private GameObject chunkTransparentPrefab;

		private GameObject target;

		public TerrainRenderer(World world, GameObject target) {
			this.world = world;
			this.target = target;
			this.chunkPrefab = Resources.Load("Prefabs/Chunk") as GameObject;
			this.chunkTransparentPrefab = Resources.Load("Prefabs/ChunkTransparent") as GameObject;
		}

		public void Init() {
			Unload ();
		}

		public void Unload() {
			foreach (var chunk in world.Chunks.Values) {
				foreach (var obj in chunk.GameObjects.Values) {
					GameObject.Destroy (obj);
				}
				chunk.GameObjects.Clear ();
			}
		}

		public bool IsDrawed(Vector3 position) {
			return IsDrawed (ChunkAddress.FromPosition (position));
		}

		public bool IsDrawed(ChunkAddress address) {
			return world.IsGenerated (address) && world [address].GameObjects.Count > 0;
		}

		public void Redraw(Vector3 position) {
			Redraw (ChunkAddress.FromPosition (position));
		}

		public void Redraw(ChunkAddress address) {
			var chunk = world [address];
			SetMesh (chunk);
		}

		public void Draw(Vector3 position) {
			Draw (ChunkAddress.FromPosition (position));
		}

		public void Draw(ChunkAddress address) {
			var chunk = world [address];

			if (!IsDrawed (address)) {
				chunk.GameObjects ["Collider"] = CreateGameObject (chunk, chunkPrefab);
				chunk.GameObjects ["OpaqueBlock"] = CreateGameObject (chunk, chunkPrefab);
				chunk.GameObjects ["TransparentBlock"] = CreateGameObject (chunk, chunkTransparentPrefab);

				SetMesh (chunk);

				chunk.GameObjects ["Collider"].SetActive (true);
				chunk.GameObjects ["OpaqueBlock"].SetActive (true);
				chunk.GameObjects ["TransparentBlock"].SetActive (true);
			}
		}

		private void SetMesh (Chunk chunk) {
			var factory = new ChunkMeshFactory (chunk);
			var mesh = factory.Create ();

			chunk.GameObjects ["Collider"].GetComponent<MeshCollider> ().sharedMesh = mesh.collider;
			chunk.GameObjects ["OpaqueBlock"].GetComponent<MeshFilter> ().mesh = mesh.opaque;
			chunk.GameObjects ["TransparentBlock"].GetComponent<MeshFilter> ().mesh = mesh.transparent;
		}

		private GameObject CreateGameObject (Chunk chunk, GameObject prefab) {
			return GameObject.Instantiate(
				prefab,
				chunk.Address.ToPosition(),
				Quaternion.identity,
				target.transform
			);
		}
	}
}