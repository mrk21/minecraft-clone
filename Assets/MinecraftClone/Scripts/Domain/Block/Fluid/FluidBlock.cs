using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block.Fluid
{
    abstract class FluidBlock : BaseBlock
    {
        public static readonly int MaxVolume = 3;
        protected int volume;
        protected bool isStream;

        public FluidBlock()
        {
            this.volume = MaxVolume;
            this.traits = BlockTraits.FluidBlock;
            this.isStream = false;
        }

        public FluidBlock(int volume) : this()
        {
            this.volume = volume;
            this.isStream = true;
        }

        public int Volume
        {
            get { return volume; }
        }

        public bool IsStream
        {
            get { return isStream; }
        }

        public abstract FluidBlock CreateStream(int volume);
    }
}