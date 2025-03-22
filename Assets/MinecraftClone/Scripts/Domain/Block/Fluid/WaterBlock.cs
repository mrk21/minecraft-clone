using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block.Fluid
{
    abstract class WaterBlock : FluidBlock
    {
        static Info info = new() {
            BlockId = 4,
            Name = nameof(WaterBlock),
            Traits = BlockTraits.FluidBlock,
        };

        static WaterBlock()
        {
            Info.Register(info);
        }

        public static Block Create()
        {
            return new Block
            {
                Id = IdGenerator.Generate(),
                BlockId = info.BlockId,
                Volume = MaxVolume,
                IsStream = false
            };
        }

        public static Block Create(int volume)
        {
            return new Block
            {
                Id = IdGenerator.Generate(),
                BlockId = info.BlockId,
                Volume = volume,
                IsStream = true
            };
        }
    }
}