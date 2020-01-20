using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    class StoneBlock : BaseBlock
    {
        public StoneBlock()
        {
            this.blockId = 3;
            this.traits = BlockTraits.SolidBlock;
        }
    }
}