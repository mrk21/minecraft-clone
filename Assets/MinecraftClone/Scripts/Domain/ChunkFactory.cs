using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Entity.Block;

namespace MinecraftClone.Domain {
	struct ChunkAddress {
		public int x;
		public int y;

		public ChunkAddress(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public static ChunkAddress FromPosition(Vector3 position) {
			return new ChunkAddress (
				Mathf.FloorToInt (position.x / 100f),
				Mathf.FloorToInt (position.z / 100f)
			);
		}

		public Vector3 ToPosition() {
			return new Vector3 (x * 100f, 0, y * 100f);
		}

		public override string ToString () {
			return string.Format ("chunk ({0}, {1})", x, y);
		}
	}

	class MapService {
		public Dictionary<ChunkAddress, ChunkFactory> chunks = new Dictionary<ChunkAddress, ChunkFactory>();
		public GameObject target;
		public GameObject waterLevel;
		public int waterHeight;

		public MapService(GameObject target, GameObject waterLevel) {
			this.target = target;
			this.waterLevel = waterLevel;
			this.waterHeight = 30;
		}

		public void Init() {
			foreach ( Transform c in target.transform ) {
				GameObject.Destroy(c.gameObject);
			}

			var realSize = 1f * 100;
			var center = realSize / 2;
			var scale = realSize / 100 * 10;

			waterLevel.transform.position = new Vector3 (center, waterHeight, center);
			waterLevel.transform.localScale = new Vector3 (scale, 1, scale);
		}

		public void Draw(Vector3 position) {
			var addr = ChunkAddress.FromPosition (position);
			ChunkFactory chunk;
			if (!chunks.TryGetValue(addr, out chunk)) {
				chunks [addr] = new ChunkFactory (addr, this);
				chunks [addr].Generate ();
				chunks [addr].Draw ();
			}
		}
	}

	class ChunkFactory {
		public static readonly int RandMax = 1000;
		public static readonly int ChunkSize = 100;

		public int?[,] heights;

		public MapService mapService;
		public ChunkAddress address;

		public int size;
		public int minHeight;
		public int maxHeight;
		public int waterHeight;
		public int resolution;
		public System.Random r;

		public ChunkFactory(ChunkAddress address, MapService mapService) {
			this.mapService = mapService;
			this.address = address;

			this.size = 20 * ChunkSize;
			this.minHeight = 0;
			this.maxHeight = 50;
			this.resolution = 20;
			this.waterHeight = mapService.waterHeight;
			this.r = new System.Random();
		}

		public void Generate() {
			heights = new int?[size, size];
			var v1 = new Vector2 (0, 0);
			var v2 = new Vector2 (size - 1, size - 1);

			ChunkFactory chunk;

			if (mapService.chunks.TryGetValue (new ChunkAddress (address.x + 1, address.y), out chunk)) {
				for (int y = 0; y < size; y++) {
					heights [size - 1, y] = chunk.heights [0, y].Value;
				}
			}
			if (mapService.chunks.TryGetValue (new ChunkAddress (address.x - 1, address.y), out chunk)) {
				for (int y = 0; y < size; y++) {
					heights [0, y] = chunk.heights [size - 1, y].Value;
				}
			}
			if (mapService.chunks.TryGetValue (new ChunkAddress (address.x, address.y + 1), out chunk)) {
				for (int x = 0; x < size; x++) {
					heights [x, size - 1] = chunk.heights [x, 0].Value;
				}
			}
			if (mapService.chunks.TryGetValue (new ChunkAddress (address.x, address.y - 1), out chunk)) {
				for (int x = 0; x < size; x++) {
					heights [x, 0] = chunk.heights [x, size - 1].Value;
				}
			}

			if (!heights [(int)v1.x, (int)v1.y].HasValue) heights [(int)v1.x, (int)v1.y] = GenerateHeight();
			if (!heights [(int)v1.x, (int)v2.y].HasValue) heights [(int)v1.x, (int)v2.y] = GenerateHeight();
			if (!heights [(int)v2.x, (int)v1.y].HasValue) heights [(int)v2.x, (int)v1.y] = GenerateHeight();
			if (!heights [(int)v2.x, (int)v2.y].HasValue) heights [(int)v2.x, (int)v2.y] = GenerateHeight();

			GenerateImpl (v1, v2, size);
		}

		public int GenerateHeight() {
			return r.Next (Mathf.RoundToInt(RandMax * 0.3f), Mathf.RoundToInt(RandMax * 0.8f));
		}

		public int NormalizePoint(int value) {
			return value / resolution;
		}

		public int NormalizeHeight(int value) {
			var result = 1f * value;
			result *= 1f * (maxHeight - minHeight) / RandMax;
			result += minHeight;
			return (int)result;
		}

		public int GenerateHeightRand(int size) {
			int value = (int)(1f * RandMax * size / this.size);
			return (int)r.Next (-Mathf.RoundToInt(value * 0.5f), Mathf.RoundToInt(value * 0.5f));
		}

		private void GenerateImpl(Vector2 v1, Vector2 v2, int size) {
			if (size <= 0) return;
			var mid = (v1 + v2) / 2;

			if (!this.heights [(int)mid.x, (int)mid.y].HasValue) {
				var value =
					heights [(int)v1.x, (int)v1.y].Value +
					heights [(int)v1.x, (int)v2.y].Value +
					heights [(int)v2.x, (int)v1.y].Value +
					heights [(int)v2.x, (int)v2.y].Value;
				value /= 4;
				value += GenerateHeightRand (size);
				if (value > RandMax) value = RandMax;
				if (value < 0) value = 0;
				this.heights [(int)mid.x, (int)mid.y] = value;
			}

			if (!heights [(int)mid.x, (int)v1.y].HasValue) {
				heights [(int)mid.x, (int)v1.y] = (
					heights [(int)v1.x, (int)v1.y] +
					heights [(int)v2.x, (int)v1.y]
				) / 2;
			}
			if (!heights [(int)v2.x, (int)mid.y].HasValue) {
				heights [(int)v2.x, (int)mid.y] = (
					heights [(int)v2.x, (int)v1.y] +
					heights [(int)v2.x, (int)v2.y]
				) / 2;
			}
			if (!heights [(int)mid.x, (int)v2.y].HasValue) {
				heights [(int)mid.x, (int)v2.y] = (
					heights [(int)v1.x, (int)v2.y] +
					heights [(int)v2.x, (int)v2.y]
				) / 2;
			}
			if (!heights [(int)v1.x, (int)mid.y].HasValue) {
				heights [(int)v1.x, (int)mid.y] = (
					heights [(int)v1.x, (int)v1.y] +
					heights [(int)v1.x, (int)v2.y]
				) / 2;
			}

			size /= 2;
			GenerateImpl (new Vector2 (v1.x, v1.y), new Vector2 (mid.x, mid.y), size);
			GenerateImpl (new Vector2(mid.x, v1.y), new Vector2(v2.x, mid.y), size);
			GenerateImpl (new Vector2(v1.x, mid.y), new Vector2(mid.x, v2.y), size);
			GenerateImpl (new Vector2(mid.x, mid.y), new Vector2(v2.x, v2.y), size);
		}

		public void Draw() {
			var basePosition = address.ToPosition();
			System.Random r = new System.Random ();

			for (int x = 0; x < this.size; x += resolution) {
				for (int y = 0; y < this.size; y += resolution) {
					var zMaxHeight = this.heights [x, y];

					if (zMaxHeight.HasValue) {
						var zMaxHeightValue = NormalizeHeight(zMaxHeight.Value);

						for (int z = zMaxHeightValue - 1; z <= zMaxHeightValue; z++) { 
							var position = basePosition + new Vector3 (NormalizePoint (x), z, NormalizePoint (y));

							var HeightRand = r.Next (0, 10);
							if (HeightRand < 2) HeightRand = -1;
							if (HeightRand > 8) HeightRand = 1;
							else HeightRand = 0;

							if (z > waterHeight + 10 + HeightRand) new StoneBlock (mapService.target, position);
							else if (z > waterHeight + 3 + HeightRand) new GrassBlock (mapService.target, position);
							else new SandBlock (mapService.target, position);
						}
					}
				}
			}
		}
	}
}
