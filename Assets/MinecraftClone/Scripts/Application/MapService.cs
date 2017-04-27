using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Renderer;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Application {
	class MapService {
		private GameObject terrain;
		private GameObject waterLevel;
		private GameObject player;
		private GameObject chunkPrefab;
		private Map map;
		private MapRenderer mapRenderer;

		public void Init(GameObject terrain, GameObject waterLevel, GameObject player, GameObject chunkPrefab) {
			this.terrain = terrain;
			this.waterLevel = waterLevel;
			this.chunkPrefab = chunkPrefab;
			this.player = player;

			if (mapRenderer != null) mapRenderer.Unload ();
			map = new Map ();
			mapRenderer = new MapRenderer (map, this.terrain, this.waterLevel, this.chunkPrefab);
			mapRenderer.Init ();
			this.player.transform.position = new Vector3 (60, ChunkFactory.MaxHeight, 60);
		}

		public Chunk CurrentChunk {
			get { return map [player.transform.position]; }
		}

		public void DrawAroundPlayer() {
			mapRenderer.Draw (player.transform.position);
		}

		public void RedrawCurrentChunk() {
			mapRenderer.Redraw(player.transform.position);
		}

		public void Redraw() {
			mapRenderer.Init ();
			DrawAroundPlayer ();
		}

		public void PutBlock(BaseBlock block, Vector3 position) {
			var chunk = map [position];
			chunk [chunk.GetLocalPosition(position)] = block;
			mapRenderer.Redraw(position);
		}
	}
}