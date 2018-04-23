using System.Collections.Generic;

namespace MinecraftClone.Domain {
	class Seed {
		private int[] seeds;

		public Seed(int baseSeed){
			var rand = new System.Random (baseSeed);
			var max = (int)System.Math.Pow (2, 16) - 1;
			seeds = new int[100];

			for (var i = 0; i < seeds.Length; i++) {
				seeds [i] = rand.Next (0, max);
			}
		}

		public int World { get { return seeds[0]; } }

		public int Temperature { get { return seeds[10]; } }
		public int Humidity    { get { return seeds[11]; } }
	}
}