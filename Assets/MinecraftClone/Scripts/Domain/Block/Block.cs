using MinecraftClone.Infrastructure;
using System;
using System.Collections.Generic;

namespace MinecraftClone.Domain.Block
{
    public struct Info {
        private static Dictionary<int, Info> data = new Dictionary<int, Info>();

        public int BlockId;
        public string Name;
        public BlockTraits Traits;

        public static void Register(Info info) {
            data[info.BlockId] = info;
        }

        public static Info Get(int blockId)
        {
            return data[blockId];
        }
    }

    public class IdGenerator {
        public static int Generate()
        {
            return (new IdGenerator()).GetHashCode();
        }

        private IdGenerator() { }
    }

    public struct Block
    {
        public int Id;
        public int BlockId;
        public int Volume;
        public bool IsStream;

        public BlockTraits Traits
        {
            get { return Info.Get(BlockId).Traits; }
        }

        public Block CreateStream(int volume)
        {
            return new Block
            {
                Id = IdGenerator.Generate(),
                BlockId = BlockId,
                Volume = volume,
                IsStream = true
            };
        }
        public override string ToString()
        {
            return string.Format("{0}#{1}", Info.Get(BlockId).Name, Id);
        }
    }
}
