using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    abstract class StoneBlock : BaseBlock
    {
        static Info info = new() {
            BlockId = 3,
            Name = nameof(StoneBlock),
            Traits = BlockTraits.SolidBlock,
        };

        static StoneBlock()
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