using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    abstract class AirBlock : BaseBlock
    {
        static Info info = new() {
            BlockId = 0,
            Name = nameof(AirBlock),
            Traits = BlockTraits.VoidBlock,
        };

        static AirBlock()
        {
            Info.Register(info);
        }

        public static Block Create()
        {
            return new() {
                Id = IdGenerator.Generate(),
                BlockId = info.BlockId,
            };
        }
    }
}