using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block.Fluid {
	class WaterBlock : FluidBlock {
		public WaterBlock() : base() {
			this.blockId = 4;
		}

		public WaterBlock(int volume) : base(volume) {
			this.blockId = 4;
		}

		public override FluidBlock CreateStream (int volume) {
			return new WaterBlock (volume);
		}
	}
}