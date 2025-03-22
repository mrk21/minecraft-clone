using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    abstract class GlassBlock
    {
        static Info info = new() {
            BlockId = 5,
            Name = nameof(GlassBlock),
            Traits = BlockTraits.TransparentSolidBlock,
        };

        static GlassBlock()
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