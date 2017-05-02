using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class AirBlock : BaseBlock {
		public AirBlock() {
			this.blockId = 0;
			this.traits = BlockTraits.VoidBlock;
		}
	}
}