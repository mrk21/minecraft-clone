using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain {
	using MatterTypeEnum = BlockTraits.MatterTypeEnum;

	class FluidPropagatorFactory {
		public FluidPropagator CreateFromSource(World world, FluidBlock source, Vector3 position) {
			return new FluidPropagator(world, source, position);
		}

		public FluidPropagator CreateFromRemovingAdjoiningBlock(World world, Vector3 basePosition) {
			return new FluidPropagator(world, basePosition);
		}
	}

	class FluidPropagator {
		private static readonly Vector3[] ExpansionDirections = new Vector3[] {
			Vector3.forward,
			Vector3.back,
			Vector3.right,
			Vector3.left
		};

		private static readonly Vector3[] AdjoiningDirections = new Vector3[] {
			Vector3.up,
			Vector3.forward,
			Vector3.back,
			Vector3.right,
			Vector3.left
		};

		public struct Item : IValueObject<Item> {
			private Vector3 position;
			private FluidBlock block;

			public Item(Vector3 position, FluidBlock block) {
				this.position = position;
				this.block = block;
			}

			public Vector3 Position { get { return position; } }
			public FluidBlock Block { get { return block; } }
		}

		private World world;
		private List<Item> edgeBlocks;
		private Dictionary<Vector3, FluidBlock> flowedBlocks;

		public FluidPropagator(World world, FluidBlock source, Vector3 position) {
			this.world = world;
			flowedBlocks = new Dictionary<Vector3, FluidBlock> ();
			edgeBlocks = new List<Item> ();
			flowedBlocks [position] = source;
			edgeBlocks.Add (new Item(position, source));
		}

		public FluidPropagator(World world, Vector3 basePosition) {
			this.world = world;
			flowedBlocks = new Dictionary<Vector3, FluidBlock> ();
			edgeBlocks = new List<Item> ();

			foreach (var direction in AdjoiningDirections) {
				var position = basePosition + direction;
				var block = world.Blocks [position];
				if (!(block is FluidBlock)) continue;
				edgeBlocks.Add (new Item (position, block as FluidBlock));
			}
		}

		public IEnumerable<List<Item>> Start() {
			while (true) {
				var nextEdgeBlocks = new List<Item> ();

				foreach (var item in edgeBlocks) {
					var fluid = item.Block;
					var position = item.Position;

					if (IsFlowable (item, Vector3.down)) {
						var nextPosition = position + Vector3.down;
						var nextStream = fluid.CreateStream (FluidBlock.MaxVolume);
						nextEdgeBlocks.Add (new Item (nextPosition, nextStream));
					}
					else if (fluid.Volume > 0) {
						foreach (var direction in ExpansionDirections) {
							if (IsFlowable (item, direction)) {
								var nextPosition = position + direction;
								var nextStream = fluid.CreateStream (fluid.Volume - 1);
								flowedBlocks [nextPosition] = nextStream;
								nextEdgeBlocks.Add (new Item (nextPosition, nextStream));
							}
						}
					}
				}
				edgeBlocks = nextEdgeBlocks;
				if (nextEdgeBlocks.Count == 0) break;
				yield return nextEdgeBlocks;
			}
		}

		private bool IsFlowable(Item item, Vector3 direction) {
			var position = item.Position + direction;
			if (flowedBlocks.ContainsKey (position)) return false;

			var block = world.Blocks [position];
			if (block == null || block.Traits.MatterType != MatterTypeEnum.Void) return false;

			if (direction != Vector3.down) {
				var downwardPosition = item.Position + Vector3.down;
				if (flowedBlocks.ContainsKey (downwardPosition)) return false;

				var obliquelyDownwardPosition = downwardPosition + direction;
				if (flowedBlocks.ContainsKey (obliquelyDownwardPosition)) return false;

				var downwardBlock = world.Blocks [downwardPosition];
				if (downwardBlock == null) return false;

				var obliquelyDownwardBlock = world.Blocks [obliquelyDownwardPosition];
				if (obliquelyDownwardBlock == null) return false;

				if (downwardBlock.Traits.MatterType != MatterTypeEnum.Solid &&
					obliquelyDownwardBlock.Traits.MatterType == MatterTypeEnum.Fluid) return false;
			}

			return true;
		}
	}
}
