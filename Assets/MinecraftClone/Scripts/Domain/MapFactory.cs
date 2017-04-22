using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Entity.Block;

namespace MinecraftClone.Domain {
	class MapFactory {
		public static readonly int RandMax = 1000;

		public int?[,] map;
		public int size;
		public int minHeight;
		public int maxHeight;
		public int waterHeight;
		public int resolution;
		public System.Random r;

		public MapFactory(int size, int minHeight, int maxHeight, int waterHeight, int resolution) {
			this.size = size;
			this.minHeight = minHeight;
			this.maxHeight = maxHeight;
			this.resolution = resolution;
			this.waterHeight = waterHeight;
			this.r = new System.Random();
		}

		public void Draw(GameObject target) {
			foreach ( Transform c in target.transform ) {
				GameObject.Destroy(c.gameObject);
			}
			System.Random r = new System.Random ();

			for (int x = 0; x < this.size; x += resolution) {
				for (int y = 0; y < this.size; y += resolution) {
					var zMaxHeight = this.map [x, y];

					if (zMaxHeight.HasValue) {
						var zMaxHeightValue = NormalizeHeight(zMaxHeight.Value);

						for (int z = zMaxHeightValue - 1; z <= zMaxHeightValue; z++) { 
							var position = new Vector3 (NormalizePoint (x), z, NormalizePoint (y));

							var HeightRand = r.Next (0, 10);
							if (HeightRand < 2) HeightRand = -1;
							if (HeightRand > 8) HeightRand = 1;
							else HeightRand = 0;

							if (z > waterHeight + 10 + HeightRand) new StoneBlock (target, position);
							else if (z > waterHeight + 3 + HeightRand) new GrassBlock (target, position);
							else new SandBlock (target, position);
						}
					}
				}
			}
		}

		public void Generate() {
			map = new int?[size, size];
			var v1 = new Vector2 (0, 0);
			var v2 = new Vector2 (size - 1, size - 1);

			map [(int)v1.x, (int)v1.y] = GenerateHeight();
			map [(int)v1.x, (int)v2.y] = GenerateHeight();
			map [(int)v2.x, (int)v1.y] = GenerateHeight();
			map [(int)v2.x, (int)v2.y] = GenerateHeight();
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

			if (!this.map [(int)mid.x, (int)mid.y].HasValue) {
				var value =
					map [(int)v1.x, (int)v1.y].Value +
					map [(int)v1.x, (int)v2.y].Value +
					map [(int)v2.x, (int)v1.y].Value +
					map [(int)v2.x, (int)v2.y].Value;
				value /= 4;
				value += GenerateHeightRand (size);
				if (value > RandMax) value = RandMax;
				if (value < 0) value = 0;
				this.map [(int)mid.x, (int)mid.y] = value;
			}

			if (!map [(int)mid.x, (int)v1.y].HasValue) {
				map [(int)mid.x, (int)v1.y] = (
					map [(int)v1.x, (int)v1.y] +
					map [(int)v2.x, (int)v1.y]
				) / 2;
			}
			if (!map [(int)v2.x, (int)mid.y].HasValue) {
				map [(int)v2.x, (int)mid.y] = (
					map [(int)v2.x, (int)v1.y] +
					map [(int)v2.x, (int)v2.y]
				) / 2;
			}
			if (!map [(int)mid.x, (int)v2.y].HasValue) {
				map [(int)mid.x, (int)v2.y] = (
					map [(int)v1.x, (int)v2.y] +
					map [(int)v2.x, (int)v2.y]
				) / 2;
			}
			if (!map [(int)v1.x, (int)mid.y].HasValue) {
				map [(int)v1.x, (int)mid.y] = (
					map [(int)v1.x, (int)v1.y] +
					map [(int)v1.x, (int)v2.y]
				) / 2;
			}

			size /= 2;
			GenerateImpl (new Vector2 (v1.x, v1.y), new Vector2 (mid.x, mid.y), size);
			GenerateImpl (new Vector2(mid.x, v1.y), new Vector2(v2.x, mid.y), size);
			GenerateImpl (new Vector2(v1.x, mid.y), new Vector2(mid.x, v2.y), size);
			GenerateImpl (new Vector2(mid.x, mid.y), new Vector2(v2.x, v2.y), size);
		}
	}
}
