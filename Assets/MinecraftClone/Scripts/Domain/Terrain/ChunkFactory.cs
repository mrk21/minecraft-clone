using UnityEngine;
using System.Collections.Generic;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;

namespace MinecraftClone.Domain.Terrain {
	class PerlinNoise {
		private class Permutation {
			private static readonly int Size = 4096; // 0x1000
			private int[] values = new int[Size];

			public Permutation (int seed) {
				var rand = new System.Random (seed);

				for (var i = 0; i < Size; i++) {
					values [i] = rand.Next (0, 255);
				}
			}

			public int this [int x] {
				get {
					return values [x & 0xfff];
				}
			}
		}

		private static readonly Dictionary<int, Permutation> Permutations = new Dictionary<int, Permutation> ();
		private Permutation p;

		public PerlinNoise(int seed) {
			if (!Permutations.ContainsKey (seed)) {
				Permutations [seed] = new Permutation (seed);
			}
			p = Permutations [seed];
		}

		public float this [float x, float y, float z] {
			get {
				int xi = (int)x & 0xf;
				int yi = (int)y & 0xf;
				int zi = (int)z & 0xf;

				float xf = x - (int)x;
				float yf = y - (int)y;
				float zf = z - (int)z;

				float u = Fade (xf);
				float v = Fade (yf);
				float w = Fade (zf);

				int aaa = p [p [p [xi]     + yi    ] + zi    ];
				int aba = p [p [p [xi]     + yi + 1] + zi    ];
				int aab = p [p [p [xi]     + yi    ] + zi + 1];
				int abb = p [p [p [xi]     + yi + 1] + zi + 1];
				int baa = p [p [p [xi + 1] + yi    ] + zi    ];
				int bba = p [p [p [xi + 1] + yi + 1] + zi    ];
				int bab = p [p [p [xi + 1] + yi    ] + zi + 1];
				int bbb = p [p [p [xi + 1] + yi + 1] + zi + 1];


				float g1 = Grad (aaa, xf    , yf    , zf);
				float g2 = Grad (baa, xf - 1, yf    , zf);
				float g3 = Grad (aba, xf    , yf - 1, zf);
				float g4 = Grad (bba, xf - 1, yf - 1, zf);

				float g5 = Grad (aab, xf    , yf    , zf - 1);
				float g6 = Grad (bab, xf - 1, yf    , zf - 1);
				float g7 = Grad (abb, xf    , yf - 1, zf - 1);
				float g8 = Grad (bbb, xf - 1, yf - 1, zf - 1);

				float x11 = Lerp (g1, g2, u);
				float x12 = Lerp (g3, g4, u);
				float y1 = Lerp (x11, x12, v);

				float x21 = Lerp (g5, g6, u);
				float x22 = Lerp (g7, g8, u);
				float y2 = Lerp (x21, x22, v);

				float z2 = (Lerp (y1, y2, w) + 1f) / 2f;
				return z2;
			}
		}

		private float Fade(float t) {
			return t * t * t * (t * (6f * t - 15f) + 10f);
		}

		private float Grad(int hash, float x, float y, float z) {
			switch (hash & 0xf) {
			case 0x0: return  x + y;
			case 0x1: return -x + y;
			case 0x2: return  x - y;
			case 0x3: return -x - y;
			case 0x4: return  x + z;
			case 0x5: return -x + z;
			case 0x6: return  x - z;
			case 0x7: return -x - z;
			case 0x8: return  y + z;
			case 0x9: return -y + z;
			case 0xa: return  y - z;
			case 0xb: return -y - z;
			case 0xc: return  y + x;
			case 0xd: return -y + z;
			case 0xe: return  y - x;
			case 0xf: return -y - z;
			default: return 0f;
			}
		}

		private float Lerp(float a, float b, float x) {
			return a + x * (b - a);
		}
	}

	class HeightMap {
		private static readonly int UnitSize = 128;
		private static readonly int MapSize = 4096;

		private PerlinNoise noise;
		private float[,] map;

		private int octaves;
		private float persistence;
		private float baseFrequency;
		private float baseAmplitude;
		private float maxAmplitude;
		private float maxHeight;

		private ChunkAddress address;

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
			for (var x = 0; x < Chunk.Size; x++) {
				for (var z = 0; z < Chunk.Size; z++) {
					map [x, z] = GetValue (x, z);
				}
			}
		}

		private float GetValue (float x, float z) {
			float result = 0;
			var offset = address.ToPosition ();
			float u = (x + offset.x + MapSize / 2f) / (1f * UnitSize);
			float w = (z + offset.z + MapSize / 2f) / (1f * UnitSize);

			for (var i = 0; i < octaves; i++) {
				float frequency = baseFrequency * Mathf.Pow (2, i);
				float amplitude = baseAmplitude * Mathf.Pow (persistence, i);

				result += amplitude * noise [
					frequency * u,
					0f,
					frequency * w
				];
			}
			result = result / maxAmplitude * maxHeight;
			return result;
		}
	}

	class ChunkFactory {
		public static readonly int MaxHeight = Chunk.Depth - 1;
		public static readonly int WaterHeight = (int)(MaxHeight * 0.5f);

		private World world;
		private ChunkAddress address;

		public ChunkFactory(World world, ChunkAddress address) {
			this.world = world;
			this.address = address;
		}

		public Chunk Create () {
			var map = new HeightMap (world.Id, address, MaxHeight);
			var chunk = new Chunk (world, address);

			map.Generate ();

			for (int x = 0; x < Chunk.Size; x++) {
				for (int z = 0; z < Chunk.Size; z++) {
					var yMax = map [x, z];
					var yMaxValue = Mathf.RoundToInt(yMax);
					if (yMaxValue > MaxHeight) yMaxValue = MaxHeight;

					for (int y = 0; y <= yMaxValue; y++) {
						BaseBlock block;
						if (y > WaterHeight + 10) block = new StoneBlock ();
						else if (y > WaterHeight + 0) block = new GrassBlock ();
						else block = new SandBlock ();

						chunk [x, y, z] = block;
					}

					for (int y = yMaxValue + 1; y < Chunk.Depth; y++) {
						BaseBlock block;
						if (y < WaterHeight) block = new WaterBlock ();
						else block = new AirBlock ();
						chunk [x, y, z] = block;
					}
				}
			}

			return chunk;
		}
	}
}