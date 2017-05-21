using UnityEngine;

namespace MinecraftClone.Domain.Terrain {
	class CavityMap {
		private static readonly int UnitSize = 128;
		private static readonly int MapSize = 65536;

		private ChunkAddress address;
		private PerlinNoise noise;
		private float[,,] map;

		private int octaves;
		private float persistence;
		private float baseFrequency;
		private float baseAmplitude;
		private float maxAmplitude;

		public CavityMap(int seed, ChunkAddress address) {
			noise = new PerlinNoise (seed);
			map = new float[Chunk.Size, Chunk.Depth, Chunk.Size];

			this.address = address;
			this.octaves = 4;
			this.persistence = 1f / 2f;
			this.baseFrequency = 1f;
			this.baseAmplitude = 1f;

			maxAmplitude = 0;

			for (var i = 0; i < octaves; i++) {
				maxAmplitude += baseAmplitude * Mathf.Pow (persistence, i);
			}
		}

		public float this[float x, float y, float z] {
			get { return map [(int)x, (int)y, (int)z]; }
		}

		public void Generate () {
			var offset = address.ToPosition ();
			float frequency = baseFrequency;
			float amplitude = baseAmplitude;

			for (var i = 0; i < octaves; i++) {
				for (var x = 0; x < Chunk.Size; x++) {
					for (var y = 0; y < Chunk.Depth; y++) {
						for (var z = 0; z < Chunk.Size; z++) {
							float u = frequency * (x + offset.x + MapSize / 2f) / UnitSize;
							float v = frequency * (y                          ) / UnitSize;
							float w = frequency * (z + offset.z + MapSize / 2f) / UnitSize;
							map [x, y, z] = amplitude * noise [u, v, w];
						}
					}
				}
				frequency *= 2f;
				amplitude *= persistence;
			}

			for (var x = 0; x < Chunk.Size; x++) {
				for (var y = 0; y < Chunk.Depth; y++) {
					for (var z = 0; z < Chunk.Size; z++) {
						map [x, y, z] /= maxAmplitude;
					}
				}
			}
		}
	}
}