using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Renderer;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain {
	class Chunk : IEntity<ChunkAddress> {
		public static readonly int Size = 50;
		public static readonly int Depth = 30;
		private static readonly BaseBlock AirBlockForOutOfRange = new AirBlock ();

		private class BlockHolder {
			private BaseBlock block;

			public BaseBlock Block {
				get {
					if (block == null) Set (new AirBlock ());
					return block;
				}
				set { Set (value); }
			}

			private void Set(BaseBlock block) {
				this.block = block;
				this.block.OnRemoveFromTerrain += OnRemove;
			}

			private void OnRemove(BaseBlock _) {
				block.OnRemoveFromTerrain -= OnRemove;
				Set (new AirBlock ());
			}
		}

		private World world;
		private ChunkAddress address;
		private ChunkFactory factory;
		private BlockHolder[,,] blocks;
		private Dictionary<string, GameObject> gameObjects;

		public Chunk(World world, ChunkAddress address, ChunkFactory factory) {
			this.world = world;
			this.address = address;
			this.factory = factory;
			this.blocks = new BlockHolder[Size, Depth, Size];
			this.gameObjects = new Dictionary<string, GameObject> ();

			for (int x = 0; x < Size; x++) {
				for (int z = 0; z < Size; z++) {
					for (int y = 0; y < Depth; y++) {
						this.blocks [x, y, z] = new BlockHolder ();
					}
				}
			}
		}

		public ChunkAddress Id {
			get { return address; }
		}

		public World World {
			get { return world; }
		}

		public ChunkFactory Factory {
			get { return factory; }
		}

		public ChunkAddress Address {
			get { return address; }
		}

		public BaseBlock this[int x, int y, int z] {
			get {
				if (y < 0 || y >= Depth) return AirBlockForOutOfRange;
				if (x < 0 || x >= Size || z < 0 || z >= Size) return null;
				return blocks [x, y, z].Block;
			}
			set { blocks [x, y, z].Block = value; }
		}

		public BaseBlock this[Vector3 position] {
			get { return this [(int)position.x, (int)position.y, (int)position.z]; }
			set { this [(int)position.x, (int)position.y, (int)position.z] = value; }
		}

		public Vector3 GetLocalPosition(Vector3 position) {
			return position - address.ToPosition ();
		}

		public Dictionary<string, GameObject> GameObjects {
			get { return gameObjects; }
		}
	}
}
