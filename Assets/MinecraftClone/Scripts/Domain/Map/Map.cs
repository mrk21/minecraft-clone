using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Map {
	class Map : IEntity<int> {
		public static readonly int WaterHeight = Chunk.Depth / 2;

		private Dictionary<ChunkAddress, Chunk> chunks;
		private int seed;
		private System.Random rand;

		public Map() {
			this.seed = GetHashCode();
			this.chunks = new Dictionary<ChunkAddress, Chunk> ();
			this.rand = new System.Random (Id);
		}

		public int Id {
			get { return seed; }
		}

		public Chunk this [Vector3 position] {
			get {
				return this [ChunkAddress.FromPosition (position)];
			}
		}

		public Chunk this [ChunkAddress address] {
			get {
				if (!IsGenerated (address)) {
					var factory = new ChunkFactory (this, address, rand);
					chunks [address] = factory.Create ();
				}
				return chunks [address];
			}
		}

		public Dictionary<ChunkAddress, Chunk> Chunks {
			get { return chunks; }
		}

		public bool IsGenerated(Vector3 position) {
			return IsGenerated (ChunkAddress.FromPosition (position));
		}

		public bool IsGenerated(ChunkAddress address) {
			return chunks.ContainsKey (address);
		}
	}
}