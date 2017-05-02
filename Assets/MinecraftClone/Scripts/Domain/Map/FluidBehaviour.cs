using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Domain.Map {
	class FluidBehaviour {
		private static readonly Vector3[] ExpansionDirections = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
		private static readonly Vector3[] AdjoiningDirections = new Vector3[] { Vector3.up, Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

		public struct QueItem {
			public Vector3 position;
			public WaterBlock block;

			public QueItem(Vector3 position, WaterBlock block) {
				this.position = position;
				this.block = block;
			}
		}

		private Chunk chunk;
		private List<QueItem> que;
		private Dictionary<Vector3, WaterBlock> blocks;

		public FluidBehaviour(Chunk chunk, Vector3 position, WaterBlock block) {
			this.chunk = chunk;
			blocks = new Dictionary<Vector3, WaterBlock> ();
			que = new List<QueItem> ();
			blocks [position] = block;
			que.Add (new QueItem(position, block));
		}

		public FluidBehaviour(Chunk chunk, Vector3 basePosition) {
			this.chunk = chunk;
			blocks = new Dictionary<Vector3, WaterBlock> ();
			que = new List<QueItem> ();

			foreach (var direction in AdjoiningDirections) {
				var position = basePosition + direction;
				var block = chunk [position];
				if (block == null || block.Traits.MatterType != BlockTraits.MatterTypeEnum.Fluid) continue;
				que.Add (new QueItem (position, block as WaterBlock));
			}
		}

		public List<QueItem> Next() {
			var result = new List<QueItem> ();

			foreach (var item in que) {
				var block = item.block;
				var position = item.position;

				if (IsTravelable (position + Vector3.down)) {
					result.Add (new QueItem (position + Vector3.down, new WaterBlock ()));
				} else if (block.Volume > 0) {
					var nextVolume = block.Volume - 1;

					foreach (var direction in ExpansionDirections) {
						if (IsTravelable (position + direction)) {
							var nextPosition = position + direction;
							var nextBlock = new WaterBlock (nextVolume);
							blocks [nextPosition] = nextBlock;
							result.Add (new QueItem (nextPosition, nextBlock));
						}
					}
				}
			}
			que = result;
			return result;
		}

		private bool IsTravelable(Vector3 position) {
			var block = chunk [position];
			if (block == null || block.Traits.MatterType != BlockTraits.MatterTypeEnum.Void) return false;
			if (blocks.ContainsKey(position)) return false;
			return true;
		}
	}
}
