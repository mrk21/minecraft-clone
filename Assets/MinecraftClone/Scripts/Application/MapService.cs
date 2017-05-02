using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Renderer;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Application {
	class MapService {
		private GameObject terrain;
		private GameObject player;
		private Map map;
		private MapRenderer mapRenderer;

		public void Init(GameObject terrain, GameObject player) {
			this.terrain = terrain;
			this.player = player;

			if (mapRenderer != null) mapRenderer.Unload ();
			map = new Map ();
			mapRenderer = new MapRenderer (map, this.terrain);
			mapRenderer.Init ();
			this.player.transform.position = new Vector3 (60, ChunkFactory.MaxHeight, 60);
		}

		public BaseBlock BlockUnderPlayer(Vector3? offset = null) {
			offset = offset ?? Vector3.zero;
			return CurrentChunk [CurrentChunk.GetLocalPosition (player.transform.position) + Vector3.down + offset.Value];
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