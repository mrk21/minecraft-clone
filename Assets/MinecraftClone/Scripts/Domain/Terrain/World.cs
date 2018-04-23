using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain;

namespace MinecraftClone.Domain.Terrain {
	public class World : IEntity<int> {
		public class BlockAccessProxy {
			private World world;

			public BlockAccessProxy(World world) {
				this.world = world;
			}

			public BaseBlock this [Vector3 position] {
				get {
					var chunk = world [position];
					return chunk [chunk.GetLocalPosition(position)];
				}
				set {
					var chunk = world [position];
					chunk [chunk.GetLocalPosition(position)] = value;
				}
			}
		}

		private BlockAccessProxy blockAccessProxy;
		private Dictionary<ChunkAddress, Chunk> chunks;
		private Seed seed;

		public World() {
			this.seed = new Seed(GetHashCode());
			this.chunks = new Dictionary<ChunkAddress, Chunk> ();
			this.blockAccessProxy = new BlockAccessProxy (this);
		}

		public Seed Seed {
			get { return seed; }
		}

		public int Id {
			get { return seed.World; }
		}

		public BlockAccessProxy Blocks {
			get { return blockAccessProxy; }
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