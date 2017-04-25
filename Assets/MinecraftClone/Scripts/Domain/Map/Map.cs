using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Map {
	class Map : IEntity<int> {
		public static readonly int WaterHeight = 30;

		private Dictionary<ChunkAddress, Chunk> chunks;
		private GameObject target;
		private GameObject waterLevel;
		private int seed;
		private System.Random rand;

		public Map(GameObject target, GameObject waterLevel) {
			this.seed = GetHashCode();
			this.chunks = new Dictionary<ChunkAddress, Chunk> ();
			this.target = target;
			this.waterLevel = waterLevel;
			this.rand = new System.Random (Id);
		}

		public Dictionary<ChunkAddress, Chunk> Chunks {
			get { return chunks; }
		}

		public GameObject Target {
			get { return target; }
		}

		public int Id {
			get { return seed; }
		}

		public void Init() {
			foreach ( Chunk chunk in chunks.Values ) {
				chunk.Unload();
			}

			var realSize = 100f;
			var center = realSize / 2f;
			var scale = realSize / 100f * 10f;

			waterLevel.transform.position = new Vector3 (center, WaterHeight, center);
			waterLevel.transform.localScale = new Vector3 (scale, 1, scale);
		}

		public bool IsGenerated(Vector3 position) {
			return IsGenerated (ChunkAddress.FromPosition (position));
		}

		public bool IsGenerated(ChunkAddress address) {
			return chunks.ContainsKey (address);
		}

		public bool IsDrawed(Vector3 position) {
			return IsDrawed (ChunkAddress.FromPosition (position));;
		}

		public bool IsDrawed(ChunkAddress address) {
			return IsGenerated (address) && chunks [address].IsDrawed ();
		}

		public void Draw(Vector3 position) {
			Draw (ChunkAddress.FromPosition (position));
		}

		public void Draw(ChunkAddress address) {
			if (!IsGenerated (address)) {
				var factory = new ChunkFactory (this, address, rand);
				chunks [address] = factory.Create ();
			}
			chunks [address].Draw ();
		}
	}
}