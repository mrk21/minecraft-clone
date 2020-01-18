using System.Collections.Generic;

namespace MinecraftClone.Domain
{
    public class Seed
    {
        private int baseSeed;
        private int[] seeds;

        public Seed()
        {
            baseSeed = GetHashCode();
            Init();
        }

        public Seed(int baseSeed)
        {
            this.baseSeed = baseSeed;
            Init();
        }

        private void Init()
        {
            var rand = new System.Random(baseSeed);
            var max = (int)System.Math.Pow(2, 16) - 1;
            seeds = new int[100];

            for (var i = 0; i < seeds.Length; i++)
            {
                seeds[i] = rand.Next(0, max);
            }
        }

        public int Base { get { return baseSeed; } }

        public int World { get { return seeds[0]; } }
        public int Temperature { get { return seeds[10]; } }
        public int Humidity { get { return seeds[11]; } }
    }
}