using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    abstract class GrassBlock : BaseBlock
    {
        static Info info = new() {
            BlockId = 1,
            Name = nameof(GrassBlock),
            Traits = BlockTraits.SolidBlock,
        };

        static GrassBlock()
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