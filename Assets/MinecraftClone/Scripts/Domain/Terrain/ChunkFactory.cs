using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;

namespace MinecraftClone.Domain.Terrain
{
    class ChunkFactory
    {
        public static readonly int MaxHeight = Chunk.Depth - 1;
        public static readonly int WaterHeight = (int)(MaxHeight * 0.5f);

        private Dimension dimension;
        private ChunkAddress address;

        public ChunkFactory(Dimension dimension, ChunkAddress address)
        {
            this.dimension = dimension;
            this.address = address;
        }

        public Chunk Create()
        {
            var chunk = new Chunk(dimension, address);
            var heightMap = new HeightMap(dimension.Seed.Dimension.Value, address, MaxHeight);
            var biomeMap = new BiomeMap(dimension.Seed.Temperature.Value, dimension.Seed.Humidity.Value, address);

            heightMap.Generate();
            biomeMap.Generate();

            for (int x = 0; x < Chunk.Size; x++)
            {
                for (int z = 0; z < Chunk.Size; z++)
                {
                    var yMax = heightMap[x, z];
                    var yMaxValue = Mathf.RoundToInt(yMax);
                    if (yMaxValue > MaxHeight) yMaxValue = MaxHeight;

                    for (int y = 0; y <= yMaxValue; y++)
                    {
                        Block.Block block;
                        if (y > WaterHeight + 10) block = StoneBlock.Create();
                        else if (y > WaterHeight + 0) block = GrassBlock.Create();
                        else block = SandBlock.Create();
                        if (y >= WaterHeight)
                        {
                            var biome = biomeMap[x, z];
                            if (biome == "desert")
                            {
                                block = SandBlock.Create();
                            }
                            else if (biome == "stone")
                            {
                                block = StoneBlock.Create();
                            }
                            else if (biome == "grass")
                            {
                                block = GrassBlock.Create();
                            }
                            else
                            {
                                block = GrassBlock.Create();
                            }
                        }
                        else
                        {
                            block = SandBlock.Create();
                        }
                        chunk[x, y, z] = block;
                    }

                    for (int y = yMaxValue + 1; y < Chunk.Depth; y++)
                    {
                        Block.Block block;
                        if (y < WaterHeight) block = WaterBlock.Create();
                        else block = AirBlock.Create();
                        chunk[x, y, z] = block;
                    }
                }
            }

            return chunk;
        }
    }
}