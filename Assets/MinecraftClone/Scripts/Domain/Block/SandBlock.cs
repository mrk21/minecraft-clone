using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    class SandBlock : BaseBlock
    {
        public SandBlock()
        {
            this.blockId = 2;
            this.traits = BlockTraits.SolidBlock;
        }
    }
}