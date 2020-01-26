using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Domain.Terrain
{
    public class Dimension : IEntity<int>
    {
        public class BlockAccessProxy
        {
            private Dimension dimension;

            public BlockAccessProxy(Dimension dimension)
            {
                this.dimension = dimension;
            }

            public BaseBlock this[Vector3 position]
            {
                get
                {
                    var chunk = dimension[position];
                    return chunk[chunk.GetLocalPosition(position)];
                }
                set
                {
                    var chunk = dimension[position];
                    chunk[chunk.GetLocalPosition(position)] = value;
                }
            }
        }

        private BlockAccessProxy blockAccessProxy;
        private Dictionary<ChunkAddress, Chunk> chunks;
        private Seed seed;

        public Dimension(Seed seed_)
        {
            this.seed = seed_;
            this.chunks = new Dictionary<ChunkAddress, Chunk>();
            this.blockAccessProxy = new BlockAccessProxy(this);
        }

        public Seed Seed
        {
            get { return seed; }
        }

        public int Id
        {
            get { return seed.Dimension.Value; }
        }

        public BlockAccessProxy Blocks
        {
            get { return blockAccessProxy; }
        }

        public Chunk this[Vector3 position]
        {
            get
            {
                return this[ChunkAddress.FromPosition(position)];
            }
        }

        public Chunk this[ChunkAddress address]
        {
            get
            {
                if (!IsGenerated(address))
                {
                    var factory = new ChunkFactory(this, address);
                    chunks[address] = factory.Create();
                }
                return chunks[address];
            }
        }

        public Dictionary<ChunkAddress, Chunk> Chunks
        {
            get { return chunks; }
        }

        public bool IsGenerated(Vector3 position)
        {
            return IsGenerated(ChunkAddress.FromPosition(position));
        }

        public bool IsGenerated(ChunkAddress address)
        {
            return chunks.ContainsKey(address);
        }
    }
}