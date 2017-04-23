using UnityEngine;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Domain.Map {
	class ChunkFactory {
		public static readonly int Size = Chunk.Size;
		public static readonly int MaxHeight = Chunk.Depth - 1;
		private static readonly int RandMax = 100000;

		class HeightMap {
			private float?[,] data;

			public HeightMap() {
				this.data = new float?[Size, Size];
			}

			public float? this[float x, float z] {
				get { return data [(int)x, (int)z]; }
				set { data [(int)x, (int)z] = value; }
			}
		}

		private Map map;
		private ChunkAddress address;
		private HeightMap heightMap;
		private System.Random r;

		public ChunkFactory(Map map, ChunkAddress address) {
			this.map = map;
			this.address = address;
			this.r = new System.Random();
		}

		public Chunk Create() {
			GenerateHightMap ();

			var chunk = new Chunk (map, address, this);

			for (int x = 0; x < Size; x++) {
				for (int z = 0; z < Size; z++) {
					var yMax = this.heightMap [x, z];

					if (yMax.HasValue) {
						var yMaxValue = Mathf.RoundToInt(yMax.Value);

						for (int y = yMaxValue - 1; y <= yMaxValue; y++) { 
							var HeightRand = GenerateHeightRand();

							BaseBlock block;
							if (y > Map.WaterHeight + 10 + HeightRand) block = new StoneBlock ();
							else if (y > Map.WaterHeight + 0 + HeightRand) block = new GrassBlock ();
							else block = new SandBlock ();
							chunk [x, y, z] = block;
						}
					}
				}
			}

			return chunk;
		}

		private void GenerateHightMap() {
			heightMap = new HeightMap ();
			var v1 = new Vector3 (0, 0, 0);
			var v2 = new Vector3 (Size - 1, 0, Size - 1);
			Chunk chunk;

			if (map.Chunks.TryGetValue (new ChunkAddress (address.x + 1, address.z), out chunk)) {
				for (int z = 0; z < Size; z++) {
					heightMap [Size - 1, z] = chunk.Factory.heightMap [0, z].Value + GenerateHeightRand();
				}
			}
			if (map.Chunks.TryGetValue (new ChunkAddress (address.x - 1, address.z), out chunk)) {
				for (int z = 0; z < Size; z++) {
					heightMap [0, z] = chunk.Factory.heightMap [Size - 1, z].Value + GenerateHeightRand();
				}
			}
			if (map.Chunks.TryGetValue (new ChunkAddress (address.x, address.z + 1), out chunk)) {
				for (int x = 0; x < Size; x++) {
					heightMap [x, Size - 1] = chunk.Factory.heightMap [x, 0].Value + GenerateHeightRand();
				}
			}
			if (map.Chunks.TryGetValue (new ChunkAddress (address.x, address.z - 1), out chunk)) {
				for (int x = 0; x < Size; x++) {
					heightMap [x, 0] = chunk.Factory.heightMap [x, Size - 1].Value + GenerateHeightRand();
				}
			}

			if (!heightMap [v1.x, v1.z].HasValue) heightMap [v1.x, v1.z] = GenerateEndHeight();
			if (!heightMap [v1.x, v2.z].HasValue) heightMap [v1.x, v2.z] = GenerateEndHeight();
			if (!heightMap [v2.x, v1.z].HasValue) heightMap [v2.x, v1.z] = GenerateEndHeight();
			if (!heightMap [v2.x, v2.z].HasValue) heightMap [v2.x, v2.z] = GenerateEndHeight();

			GenerateHightMapImpl (v1, v2, Size);
		}

		private void GenerateHightMapImpl(Vector3 v1, Vector3 v2, int size) {
			if (size <= 0) return;
			var mid = (v1 + v2) / 2f;

			if (!this.heightMap [mid.x, mid.z].HasValue) {
				var value =
					heightMap [v1.x, v1.z].Value +
					heightMap [v1.x, v2.z].Value +
					heightMap [v2.x, v1.z].Value +
					heightMap [v2.x, v2.z].Value;
				value /= 4f;
				value += GenerateMidHeightRand (size);
				if (value > MaxHeight) value = MaxHeight;
				if (value < 0) value = 0;
				this.heightMap [mid.x, mid.z] = value;
			}

			if (!heightMap [mid.x, v1.z].HasValue) {
				heightMap [mid.x, v1.z] = (
					heightMap [v1.x, v1.z] +
					heightMap [v2.x, v1.z]
				) / 2;
			}
			if (!heightMap [v2.x, mid.z].HasValue) {
				heightMap [v2.x, mid.z] = (
					heightMap [v2.x, v1.z] +
					heightMap [v2.x, v2.z]
				) / 2;
			}
			if (!heightMap [mid.x, v2.z].HasValue) {
				heightMap [mid.x, v2.z] = (
					heightMap [v1.x, v2.z] +
					heightMap [v2.x, v2.z]
				) / 2;
			}
			if (!heightMap [v1.x, mid.z].HasValue) {
				heightMap [v1.x, mid.z] = (
					heightMap [v1.x, v1.z] +
					heightMap [v1.x, v2.z]
				) / 2;
			}

			size /= 2;
			GenerateHightMapImpl (new Vector3 (v1.x, 0, v1.z), new Vector3 (mid.x, 0, mid.z), size);
			GenerateHightMapImpl (new Vector3 (mid.x, 0, v1.z), new Vector3 (v2.x, 0, mid.z), size);
			GenerateHightMapImpl (new Vector3 (v1.x, 0, mid.z), new Vector3 (mid.x, 0, v2.z), size);
			GenerateHightMapImpl (new Vector3 (mid.x, 0, mid.z), new Vector3 (v2.x, 0, v2.z), size);
		}

		private float GenerateEndHeight() {
			return 1f * r.Next (0, RandMax) / RandMax * MaxHeight;
		}

		private float GenerateMidHeightRand(int size) {
			var value = Mathf.RoundToInt (RandMax / 3f);
			return 1f * r.Next (-value, value) / (2f * value) * MaxHeight * size / Size;
		}

		private float GenerateHeightRand() {
			var result = r.Next (0, 10);
			if (result < 2) return -1;
			if (result > 8) return 1;
			else return 0;
		}
	}
}