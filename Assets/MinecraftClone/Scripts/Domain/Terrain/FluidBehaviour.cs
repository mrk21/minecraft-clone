using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain {
	class FluidBehaviour {
		private static readonly Vector3[] ExpansionDirections = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
		private static readonly Vector3[] AdjoiningDirections = new Vector3[] { Vector3.up, Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

		public struct QueItem : IValueObject<QueItem> {
			private Vector3 position;
			private FluidBlock block;

			public QueItem(Vector3 position, FluidBlock block) {
				this.position = position;
				this.block = block;
			}

			public Vector3 Position { get { return position; } }
			public FluidBlock Block { get { return block; } }
		}

		private Chunk chunk;
		private List<QueItem> que;
		private Dictionary<Vector3, FluidBlock> blocks;

		public FluidBehaviour(Chunk chunk, Vector3 position, FluidBlock block) {
			this.chunk = chunk;
			blocks = new Dictionary<Vector3, FluidBlock> ();
			que = new List<QueItem> ();
			blocks [position] = block;
			que.Add (new QueItem(position, block));
		}

		public FluidBehaviour(Chunk chunk, Vector3 basePosition) {
			this.chunk = chunk;
			blocks = new Dictionary<Vector3, FluidBlock> ();
			que = new List<QueItem> ();

			foreach (var direction in AdjoiningDirections) {
				var position = basePosition + direction;
				var block = chunk [position];
				if (!(block is FluidBlock)) continue;
				que.Add (new QueItem (position, block as FluidBlock));
			}
		}

		public List<QueItem> Next() {
			var result = new List<QueItem> ();

			foreach (var item in que) {
				var block = item.Block;
				var position = item.Position;

				if (IsTravelable (item, Vector3.down)) {
					result.Add (new QueItem (position + Vector3.down, block.MakeStream (FluidBlock.MaxVolume)));
				} else if (block.Volume > 0) {
					foreach (var direction in ExpansionDirections) {
						if (IsTravelable (item, direction)) {
							var nextPosition = position + direction;
							var nextBlock = block.MakeStream (block.Volume - 1);
							blocks [nextPosition] = nextBlock;
							result.Add (new QueItem (nextPosition, nextBlock));
						}
					}
				}
			}
			que = result;
			return result;
		}

		private bool IsTravelable(QueItem item, Vector3 direction) {
			var position = item.Position + direction;
			var block = chunk [position];
			if (block == null || block.Traits.MatterType != BlockTraits.MatterTypeEnum.Void) return false;

			if (blocks.ContainsKey(position)) return false;

			if (direction != Vector3.down) {
				var downwardBlock = chunk [item.Position + Vector3.down];
				var obliquelyDownwardBlock = chunk [item.Position + direction + Vector3.down];

				if (downwardBlock == null || obliquelyDownwardBlock == null) return false;
				if (downwardBlock.Traits.MatterType != BlockTraits.MatterTypeEnum.Solid &&
					obliquelyDownwardBlock.Traits.MatterType == BlockTraits.MatterTypeEnum.Fluid) return false;
			}

			return true;
		}
	}
}
