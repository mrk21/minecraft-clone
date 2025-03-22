using System;
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
        private static readonly Block.Block AirBlockForOutOfRange = AirBlock.Create();

        private Dimension dimension;
        private ChunkAddress address;
        private Block.Block[,,] blocks;
        private Dictionary<string, GameObject> gameObjects;

        public Chunk(Dimension dimension, ChunkAddress address)
        {
            this.dimension = dimension;
            this.address = address;
            this.blocks = new Block.Block[Size, Depth, Size];
            this.gameObjects = new Dictionary<string, GameObject>();
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

        public Block.Block this[int x, int y, int z]
        {
            get
            {
                if (y < 0 || y >= Depth) return AirBlockForOutOfRange;
                if (x < 0 || x >= Size || z < 0 || z >= Size) return new Block.Block { BlockId = -1 };
                return blocks[x, y, z];
            }
            set
            { blocks[x, y, z] = value; }
        }

        public Block.Block this[Vector3 position]
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
