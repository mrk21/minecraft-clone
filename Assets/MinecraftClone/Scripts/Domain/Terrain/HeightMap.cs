using UnityEngine;

namespace MinecraftClone.Domain.Terrain {
	class HeightMap {
		private static readonly int UnitSize = 128;
		private static readonly int MapSize = 65536;

		private ChunkAddress address;
		private PerlinNoise noise;
		private float[,] map;

		private int octaves;
		private float persistence;
		private float baseFrequency;
		private float baseAmplitude;
		private float maxAmplitude;
		private float maxHeight;

		public HeightMap(int seed, ChunkAddress address, float maxHeight) {
			noise = new PerlinNoise (seed);
			map = new float[Chunk.Size, Chunk.Size];

			this.address = address;
			this.octaves = 4;
			this.persistence = 1f / 2f;
			this.baseFrequency = 1f;
			this.baseAmplitude = maxHeight * 0.75f;
			this.maxHeight = maxHeight;

			maxAmplitude = 0;

			for (var i = 0; i < octaves; i++) {
				maxAmplitude += baseAmplitude * Mathf.Pow (persistence, i);
			}
		}

		public float this[float x, float z] {
			get { return map [(int)x, (int)z]; }
		}

		public void Generate () {
			var offset = address.ToPosition ();
			float frequency = baseFrequency;
			float amplitude = baseAmplitude;

			for (var i = 0; i < octaves; i++) {
				for (var x = 0; x < Chunk.Size; x++) {
					for (var z = 0; z < Chunk.Size; z++) {
						float u = frequency * (x + offset.x + MapSize / 2f) / UnitSize;
						float w = frequency * (z + offset.z + MapSize / 2f) / UnitSize;
						map [x, z] += amplitude * noise [u, 0f, w];
					}
				}
				frequency *= 2f;
				amplitude *= persistence;
			}

			for (var x = 0; x < Chunk.Size; x++) {
				for (var z = 0; z < Chunk.Size; z++) {
					map [x, z] /= maxAmplitude;
					map [x, z] *= maxHeight;
				}
			}
		}
	}
}