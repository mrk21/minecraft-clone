using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain {
	class World : IEntity<int> {
		private Dictionary<ChunkAddress, Chunk> chunks;
		private int seed;

		public World() {
			this.seed = GetHashCode();
			this.chunks = new Dictionary<ChunkAddress, Chunk> ();
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
					var factory = new ChunkFactory (this, address);
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