using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class GlassBlock : BaseBlock {
		public GlassBlock() {
			this.blockId = 5;
			this.traits = BlockTraits.TransparentSolidBlock;
		}
	}
}