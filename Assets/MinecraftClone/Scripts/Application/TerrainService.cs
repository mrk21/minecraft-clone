using UnityEngine;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Renderer;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Application {
	class TerrainService {
		private GameObject terrain;
		private GameObject player;
		private World world;
		private TerrainRenderer terrainRenderer;

		public void Init(GameObject terrain, GameObject player) {
			this.terrain = terrain;
			this.player = player;

			if (terrainRenderer != null) terrainRenderer.Unload ();
			world = new World ();
			terrainRenderer = new TerrainRenderer (world, this.terrain);
			terrainRenderer.Init ();
			this.player.transform.position = new Vector3 (60, ChunkFactory.MaxHeight, 60);
		}

		public BaseBlock BlockUnderPlayer(Vector3? offset = null) {
			offset = offset ?? Vector3.zero;
			return CurrentChunk [CurrentChunk.GetLocalPosition (player.transform.position) + Vector3.down + offset.Value];
		}

		public Chunk CurrentChunk {
			get { return world [player.transform.position]; }
		}

		public void DrawAroundPlayer(int xOffset = 0, int zOffset = 0) {
			var baseAddress = ChunkAddress.FromPosition (player.transform.position);
			var address = new ChunkAddress (baseAddress.X + xOffset, baseAddress.Z + zOffset);
			terrainRenderer.Draw (address);
		}

		public void RedrawCurrentChunk() {
			terrainRenderer.Redraw(player.transform.position);
		}

		public void Redraw() {
			terrainRenderer.Init ();
			DrawAroundPlayer ();
		}

		public void PutBlock(BaseBlock block, Vector3 position) {
			var chunk = world [position];
			chunk [chunk.GetLocalPosition(position)] = block;
			terrainRenderer.Redraw(position);
		}
	}
}