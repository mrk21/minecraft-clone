using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Map {
	class Chunk : IEntity<ChunkAddress> {
		public static readonly int Size = 100;
		public static readonly int Depth = 50;

		private struct BlockHolder {
			private BaseBlock block;

			public BaseBlock Block {
				get { return block; }
				set {
					block = value;
					block.OnRemoveFromTerrain += OnRemove;
				}
			}

			private void OnRemove(BaseBlock _) {
				block.OnRemoveFromTerrain -= OnRemove;
				block = null;
			}
		}

		private Map map;
		private ChunkAddress address;
		private ChunkFactory factory;
		private BlockHolder[,,] blocks;

		public ChunkAddress Id {
			get { return address; }
		}

		public Chunk(Map map, ChunkAddress address, ChunkFactory factory) {
			this.map = map;
			this.address = address;
			this.factory = factory;
			this.blocks = new BlockHolder[Size, Depth, Size];
		}

		public ChunkFactory Factory {
			get { return factory; }
		}

		public ChunkAddress Address {
			get { return address; }
		}

		public BaseBlock this[int x, int y, int z] {
			get { return blocks [x, y, z].Block; }
			set { blocks [x, y, z].Block = value; }
		}

		public BaseBlock this[Vector3 position] {
			get { return blocks [(int)position.x, (int)position.y, (int)position.z].Block; }
			set { blocks [(int)position.x, (int)position.y, (int)position.z].Block = value; }
		}

		public Vector3 GetLocalPosition(Vector3 position) {
			return position - address.ToPosition ();
		}

		public void Draw() {
			var basePosition = Address.ToPosition();

			for (int x = 0; x < Size; x++) {
				for (int z = 0; z < Size; z++) {
					for (int y = 0; y < Depth; y++) { 
						BaseBlock block = this[x, y, z];

						if (block != null) {
							var position = new Vector3 (x, y, z);
							position += basePosition;
							block.Draw (map.Target, position);
						}
					}
				}
			}
		}
	}
}
