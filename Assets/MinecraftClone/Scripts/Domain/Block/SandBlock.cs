using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    abstract class SandBlock : BaseBlock
    {
        static Info info = new() {
            BlockId = 2,
            Name = nameof(SandBlock),
            Traits = BlockTraits.SolidBlock,
        };

        static SandBlock()
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