using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class WaterBlock : BaseBlock {
		public WaterBlock() {
			this.blockId = 4;
			this.traits = BlockTraits.FluidBlock;
		}
	}
}