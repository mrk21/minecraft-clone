using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class AirBlock : BaseBlock {
		public override int BlockId {
			get { return 0; }
		}

		public override bool IsTransparent {
			get { return true; }
		}

		public override bool IsVoid {
			get { return true; }
		}
	}
}