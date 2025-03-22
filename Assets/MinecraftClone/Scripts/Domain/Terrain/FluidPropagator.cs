using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain
{
    using MatterTypeEnum = BlockTraits.MatterTypeEnum;

    class FluidPropagatorFactory
    {
        public FluidPropagator CreateFromSource(Dimension dimension, Block.Block source, Vector3 position)
        {
            return new FluidPropagator(dimension, source, position);
        }

        public FluidPropagator CreateFromRemovingAdjoiningBlock(Dimension dimension, Vector3 basePosition)
        {
            return new FluidPropagator(dimension, basePosition);
        }
    }

    class FluidPropagator
    {
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

        public struct Item : IValueObject<Item>
        {
            private Vector3 position;
            private Block.Block block;

            public Item(Vector3 position, Block.Block block)
            {
                this.position = position;
                this.block = block;
            }

            public Vector3 Position { get { return position; } }
            public Block.Block Block { get { return block; } }
        }

        private Dimension dimension;
        private List<Item> edgeBlocks;
        private Dictionary<Vector3, Block.Block> flowedBlocks;

        public FluidPropagator(Dimension dimension, Block.Block source, Vector3 position)
        {
            this.dimension = dimension;
            flowedBlocks = new Dictionary<Vector3, Block.Block>();
            edgeBlocks = new List<Item>();
            flowedBlocks[position] = source;
            edgeBlocks.Add(new Item(position, source));
        }

        public FluidPropagator(Dimension dimension, Vector3 basePosition)
        {
            this.dimension = dimension;
            flowedBlocks = new Dictionary<Vector3, Block.Block>();
            edgeBlocks = new List<Item>();

            foreach (var direction in AdjoiningDirections)
            {
                var position = basePosition + direction;
                var block = dimension.Blocks[position];
                if (block.Traits.MatterType != MatterTypeEnum.Fluid) continue;
                edgeBlocks.Add(new Item(position, block));
            }
        }

        public IEnumerable<List<Item>> Start()
        {
            while (true)
            {
                var nextEdgeBlocks = new List<Item>();

                foreach (var item in edgeBlocks)
                {
                    var fluid = item.Block;
                    var position = item.Position;

                    if (IsFlowable(item, Vector3.down))
                    {
                        var nextPosition = position + Vector3.down;
                        var nextStream = fluid.CreateStream(FluidBlock.MaxVolume);
                        nextEdgeBlocks.Add(new Item(nextPosition, nextStream));
                    }
                    else if (fluid.Volume > 0)
                    {
                        foreach (var direction in ExpansionDirections)
                        {
                            if (IsFlowable(item, direction))
                            {
                                var nextPosition = position + direction;
                                var nextStream = fluid.CreateStream(fluid.Volume - 1);
                                flowedBlocks[nextPosition] = nextStream;
                                nextEdgeBlocks.Add(new Item(nextPosition, nextStream));
                            }
                        }
                    }
                }
                edgeBlocks = nextEdgeBlocks;
                if (nextEdgeBlocks.Count == 0) break;
                yield return nextEdgeBlocks;
            }
        }

        private bool IsFlowable(Item item, Vector3 direction)
        {
            var position = item.Position + direction;
            if (flowedBlocks.ContainsKey(position)) return false;

            var block = dimension.Blocks[position];
            if (block.BlockId == -1 || block.Traits.MatterType != MatterTypeEnum.Void) return false;

            if (direction != Vector3.down)
            {
                var downwardPosition = item.Position + Vector3.down;
                if (flowedBlocks.ContainsKey(downwardPosition)) return false;

                var obliquelyDownwardPosition = downwardPosition + direction;
                if (flowedBlocks.ContainsKey(obliquelyDownwardPosition)) return false;

                var downwardBlock = dimension.Blocks[downwardPosition];
                if (downwardBlock.BlockId == -1) return false;

                var obliquelyDownwardBlock = dimension.Blocks[obliquelyDownwardPosition];
                if (obliquelyDownwardBlock.BlockId == -1) return false;

                if (downwardBlock.Traits.MatterType != MatterTypeEnum.Solid &&
                    obliquelyDownwardBlock.Traits.MatterType == MatterTypeEnum.Fluid) return false;
            }

            return true;
        }
    }
}
