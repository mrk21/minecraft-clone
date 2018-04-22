using System;
using System.Collections.Generic;

namespace MinecraftClone.Domain.Noise {
	class OctavePerlinNoise {
		private PerlinNoise noise;
		private int octave;
		private float[] frequencies;
		private float[] amplitudes;
		private float compAmplitude;

		public OctavePerlinNoise(int seed, int octave_) {
			noise = new PerlinNoise (seed);
			octave = octave_;

			frequencies = new float[octave];
			amplitudes = new float[octave];
			compAmplitude = 0.0f;

			for (int i = 0; i < octave; i++) {
				frequencies [i] = (float)Math.Pow (2, i);
				amplitudes [i] = (float)Math.Pow (2, -i);
				compAmplitude += amplitudes [i];
			}
		}

		public float this [float x, float y, float z] {
			get {
				float w = 0.0f;

				for (int i = 0; i < octave; i++) {
					w += amplitudes[i] * noise [frequencies[i] * x, frequencies[i] * y, frequencies[i] * z];
				}
				return w / compAmplitude;
			}
		}
	}
}