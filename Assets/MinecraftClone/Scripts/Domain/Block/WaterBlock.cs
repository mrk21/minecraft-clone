using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class WaterBlock : BaseBlock {
		public static readonly int MaxVolume = 3;
		private int volume;

		public WaterBlock() {
			this.blockId = 4;
			this.volume = MaxVolume;
			this.traits = BlockTraits.FluidBlock;
		}

		public WaterBlock(int volume) : this() {
			this.volume = volume;
		}


		public int Volume {
			get { return volume; }
		}
	}
}