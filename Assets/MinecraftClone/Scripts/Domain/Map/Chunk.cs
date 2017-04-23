using UnityEngine;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Domain.Map {
	class Chunk {
		public static readonly int Size = 100;
		public static readonly int Depth = 50;

		private Map map;
		private ChunkAddress address;
		private ChunkFactory factory;
		private BaseBlock[,,] blocks;

		public Chunk(Map map, ChunkAddress address, ChunkFactory factory) {
			this.map = map;
			this.address = address;
			this.factory = factory;
			this.blocks = new BaseBlock[Size, Depth, Size];
		}

		public ChunkFactory Factory {
			get { return factory; }
		}

		public ChunkAddress Address {
			get { return address; }
		}

		public BaseBlock this[int x, int y, int z] {
			get { return blocks [x, y, z]; }
			set { blocks [x, y, z] = value; }
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