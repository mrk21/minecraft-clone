using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Map {
	class Chunk : IEntity<ChunkAddress> {
		public static readonly int Size = 100;
		public static readonly int Depth = 50;

		private class BlockHolder {
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
		private bool isDrawed;

		public Chunk(Map map, ChunkAddress address, ChunkFactory factory) {
			this.map = map;
			this.address = address;
			this.factory = factory;
			this.isDrawed = false;
			this.blocks = new BlockHolder[Size, Depth, Size];

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
			get { return this [(int)position.x, (int)position.y, (int)position.z]; }
			set { this [(int)position.x, (int)position.y, (int)position.z] = value; }
		}

		public Vector3 GetLocalPosition(Vector3 position) {
			return position - address.ToPosition ();
		}

		public bool IsDrawed() {
			return isDrawed;
		}

		public void Unload () {
			for (int x = 0; x < Size; x++) {
				for (int z = 0; z < Size; z++) {
					for (int y = 0; y < Depth; y++) {
						BaseBlock block = this[x, y, z];

						if (block != null) block.Unload();
					}
				}
			}
			isDrawed = false;
		}

		public void Draw() {
			if (IsDrawed ()) return;

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

			isDrawed = true;
		}
	}
}
