using UnityEngine;
using MinecraftClone.Domain.Noise;

namespace MinecraftClone.Domain.Terrain {
	class BiomeMap {
		private static readonly int UnitSize = 1024;
		private static readonly int MapSize = 65536;

		class TemperatureMap {
			private OctavePerlinNoise noise;

			public TemperatureMap(int seed) {
				noise = new OctavePerlinNoise (seed, 8);
			}

			public int this[float x, float z] {
				get {
					return (int)System.Math.Round(50 * noise [x, 0f, z]);
				}
			}
		}

		class HumidityMap {
			private OctavePerlinNoise noise;

			public HumidityMap(int seed) {
				noise = new OctavePerlinNoise (seed, 8);
			}

			public int this[float x, float z] {
				get {
					return (int)System.Math.Round(100 * noise [x, 0f, z]);
				}
			}
		}

		private ChunkAddress address;
		private TemperatureMap temperatureMap;
		private HumidityMap humidityMap;
		private string[,] map;

		public BiomeMap(int temperatureSeed, int humiditySeed, ChunkAddress address) {
			temperatureMap = new TemperatureMap(temperatureSeed);
			humidityMap = new HumidityMap(humiditySeed);
			map = new string[Chunk.Size, Chunk.Size];
			this.address = address;
		}

		public string this[float x, float z] {
			get { return map [(int)x, (int)z]; }
		}

		public void Generate () {
			var offset = address.ToPosition ();

			for (var x = 0; x < Chunk.Size; x++) {
				for (var z = 0; z < Chunk.Size; z++) {
					float u = (x + offset.x + MapSize / 2f) / UnitSize;
					float w = (z + offset.z + MapSize / 2f) / UnitSize;
					int temperature = temperatureMap [u, w];
					int humidity = humidityMap [u, w];

					if (temperature > 30) {
						map [x, z] = "desert";
					} else {
						if (humidity < 50) {
							map [x, z] = "stone";
						} else {
							map [x, z] = "grass";
						}
					}
				}
			}
		}
	}
}