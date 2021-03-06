using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Block;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain
{
    public class Chunk : IEntity<ChunkAddress>
    {
        public static readonly int Size = 16;
        public static readonly int Depth = 128;
        private static readonly BaseBlock AirBlockForOutOfRange = new AirBlock();

        private class BlockHolder
        {
            private BaseBlock block;

            public BaseBlock Block
            {
                get
                {
                    if (block == null) Set(new AirBlock());
                    return block;
                }
                set { Set(value); }
            }

            private void Set(BaseBlock block)
            {
                this.block = block;
                this.block.OnRemoveFromTerrain += OnRemove;
            }

            private void OnRemove(BaseBlock _)
            {
                block.OnRemoveFromTerrain -= OnRemove;
                Set(new AirBlock());
            }
        }

        private Dimension dimension;
        private ChunkAddress address;
        private BlockHolder[,,] blocks;
        private Dictionary<string, GameObject> gameObjects;

        public Chunk(Dimension dimension, ChunkAddress address)
        {
            this.dimension = dimension;
            this.address = address;
            this.blocks = new BlockHolder[Size, Depth, Size];
            this.gameObjects = new Dictionary<string, GameObject>();

            for (int x = 0; x < Size; x++)
            {
                for (int z = 0; z < Size; z++)
                {
                    for (int y = 0; y < Depth; y++)
                    {
                        this.blocks[x, y, z] = new BlockHolder();
                    }
                }
            }
        }

        public ChunkAddress Id
        {
            get { return address; }
        }

        public Dimension Dimension
        {
            get { return dimension; }
        }

        public ChunkAddress Address
        {
            get { return address; }
        }

        public BaseBlock this[int x, int y, int z]
        {
            get
            {
                if (y < 0 || y >= Depth) return AirBlockForOutOfRange;
                if (x < 0 || x >= Size || z < 0 || z >= Size) return null;
                return blocks[x, y, z].Block;
            }
            set
            {
                blocks[x, y, z].Block = value;
            }
        }

        public BaseBlock this[Vector3 position]
        {
            get { return this[(int)position.x, (int)position.y, (int)position.z]; }
            set { this[(int)position.x, (int)position.y, (int)position.z] = value; }
        }

        public Vector3 GetLocalPosition(Vector3 position)
        {
            return position - address.ToPosition();
        }

        public Dictionary<string, GameObject> GameObjects
        {
            get { return gameObjects; }
        }
    }
}
