using UnityEngine;
using MinecraftClone.Domain.Noise;

namespace MinecraftClone.Domain.Terrain {
	class HeightMap {
		private static readonly int UnitSize = 256;
		private static readonly int MapSize = 65536;

		private ChunkAddress address;
		private OctavePerlinNoise noise;
		private float[,] map;
		private float maxHeight;

		public HeightMap(int seed, ChunkAddress address, float maxHeight) {
			noise = new OctavePerlinNoise (seed, 6);
			map = new float[Chunk.Size, Chunk.Size];

			this.address = address;
			this.maxHeight = maxHeight;
		}

		public float this[float x, float z] {
			get { return map [(int)x, (int)z]; }
		}

		public void Generate () {
			var offset = address.ToPosition ();

			for (var x = 0; x < Chunk.Size; x++) {
				for (var z = 0; z < Chunk.Size; z++) {
					float u = (x + offset.x + MapSize / 2f) / UnitSize;
					float w = (z + offset.z + MapSize / 2f) / UnitSize;
					map [x, z] = maxHeight * noise [u, 0f, w];
				}
			}
		}
	}
}