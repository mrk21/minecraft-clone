using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Domain.Block;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Application {
	class MapService {
		private GameObject terrain;
		private GameObject waterLevel;
		private GameObject player;
		private Map map;

		public void Init(GameObject terrain, GameObject waterLevel, GameObject player) {
			this.terrain = terrain;
			this.waterLevel = waterLevel;
			this.player = player;

			map = new Map (this.terrain, this.waterLevel);
			map.Init ();
			player.transform.position = new Vector3 (60, ChunkFactory.MaxHeight, 60);
		}

		public void DrawAroundPlayer() {
			map.Draw (player.transform.position);
		}

		public void Redraw() {
			map.Init ();
			DrawAroundPlayer ();
		}

		public void PutBlock(BaseBlock block, Vector3 position) {
			var chunk = map.Chunks [ChunkAddress.FromPosition (position)];
			chunk [chunk.GetLocalPosition(position)] = block;
			block.Draw (terrain, position);
		}
	}
}