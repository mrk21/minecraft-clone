using System.Collections.Generic;

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
				get { return values [x & 0xfff]; }
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
				int xi = (int)x;
				int yi = (int)y;
				int zi = (int)z;

				float xf = x - xi;
				float yf = y - yi;
				float zf = z - zi;

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

				float r = (Lerp (y1, y2, w) + 1f) / 2f;
				return r;
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
}