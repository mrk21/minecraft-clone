using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Map {
	class Map : IEntity<int> {
		public static readonly int WaterHeight = 30;

		private Dictionary<ChunkAddress, Chunk> chunks;
		private GameObject target;
		private GameObject waterLevel;
		private int id;

		public Map(GameObject target, GameObject waterLevel) {
			this.id = GetHashCode();
			this.chunks = new Dictionary<ChunkAddress, Chunk> ();
			this.target = target;
			this.waterLevel = waterLevel;
		}

		public Dictionary<ChunkAddress, Chunk> Chunks {
			get { return chunks; }
		}

		public GameObject Target {
			get { return target; }
		}

		public int Id {
			get { return id; }
		}

		public void Init() {
			foreach ( Transform c in target.transform ) {
				GameObject.Destroy(c.gameObject);
			}

			var realSize = 100f;
			var center = realSize / 2f;
			var scale = realSize / 100f * 10f;

			waterLevel.transform.position = new Vector3 (center, WaterHeight, center);
			waterLevel.transform.localScale = new Vector3 (scale, 1, scale);
		}

		public void Draw(Vector3 position) {
			var addr = ChunkAddress.FromPosition (position);
			Chunk chunk;
			if (!chunks.TryGetValue(addr, out chunk)) {
				var factory = new ChunkFactory (this, addr);
				chunk = factory.Create ();
				chunks [addr] = chunk;
				chunk.Draw ();
			}
		}
	}
}