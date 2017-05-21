using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;

namespace MinecraftClone.Domain.Terrain {
	class ChunkFactory {
		public static readonly float CavityThreshold = 0.026f;
		public static readonly int MaxHeight = Chunk.Depth - 1;
		public static readonly int WaterHeight = (int)(MaxHeight * 0.5f);

		private World world;
		private ChunkAddress address;

		public ChunkFactory(World world, ChunkAddress address) {
			this.world = world;
			this.address = address;
		}

		public Chunk Create () {
			var chunk = new Chunk (world, address);
			var heightMap = new HeightMap (world.Id, address, MaxHeight);
			var cavityMap = new CavityMap (world.Id, address);

			heightMap.Generate ();
			cavityMap.Generate ();

			for (int x = 0; x < Chunk.Size; x++) {
				for (int z = 0; z < Chunk.Size; z++) {
					var yMax = heightMap [x, z];
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

					for (int y = 5; y < Chunk.Depth; y++) {
						if (chunk [x, y  , z] is WaterBlock) continue;
						if (chunk [x, y+1, z] is WaterBlock) continue;
						if (cavityMap [x, y, z] < CavityThreshold) {
							chunk [x, y, z] = new AirBlock ();
						}
					}
				}
			}

			return chunk;
		}
	}
}