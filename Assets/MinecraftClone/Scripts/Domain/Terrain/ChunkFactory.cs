using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;

namespace MinecraftClone.Domain.Terrain
{
    class ChunkFactory
    {
        public static readonly int MaxHeight = Chunk.Depth - 1;
        public static readonly int WaterHeight = (int)(MaxHeight * 0.5f);

        private World world;
        private ChunkAddress address;

        public ChunkFactory(World world, ChunkAddress address)
        {
            this.world = world;
            this.address = address;
        }

        public Chunk Create()
        {
            var chunk = new Chunk(world, address);
            var heightMap = new HeightMap(world.Seed.World, address, MaxHeight);
            var biomeMap = new BiomeMap(world.Seed.Temperature, world.Seed.Humidity, address);

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
                        BaseBlock block;
                        if (y > WaterHeight + 10) block = new StoneBlock();
                        else if (y > WaterHeight + 0) block = new GrassBlock();
                        else block = new SandBlock();
                        if (y >= WaterHeight)
                        {
                            var biome = biomeMap[x, z];
                            if (biome == "desert")
                            {
                                block = new SandBlock();
                            }
                            else if (biome == "stone")
                            {
                                block = new StoneBlock();
                            }
                            else if (biome == "grass")
                            {
                                block = new GrassBlock();
                            }
                            else
                            {
                                block = new GrassBlock();
                            }
                        }
                        else
                        {
                            block = new SandBlock();
                        }
                        chunk[x, y, z] = block;
                    }

                    for (int y = yMaxValue + 1; y < Chunk.Depth; y++)
                    {
                        BaseBlock block;
                        if (y < WaterHeight) block = new WaterBlock();
                        else block = new AirBlock();
                        chunk[x, y, z] = block;
                    }
                }
            }

            return chunk;
        }
    }
}