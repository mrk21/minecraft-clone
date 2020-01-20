using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    class GrassBlock : BaseBlock
    {
        public GrassBlock()
        {
            this.blockId = 1;
            this.traits = BlockTraits.SolidBlock;
        }
    }
}