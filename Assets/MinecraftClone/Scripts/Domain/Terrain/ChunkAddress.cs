using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Terrain {
	public struct ChunkAddress : IValueObject<ChunkAddress> {
		private int x;
		private int z;

		public ChunkAddress(int x, int z) {
			this.x = x;
			this.z = z;
		}

		public int X { get { return x; } }
		public int Z { get { return z; } }

		public static ChunkAddress FromPosition(Vector3 position) {
			return new ChunkAddress (
				Mathf.FloorToInt (1f * position.x / Chunk.Size),
				Mathf.FloorToInt (1f * position.z / Chunk.Size)
			);
		}

		public Vector3 ToPosition() {
			return new Vector3 (X * Chunk.Size, 0f, Z * Chunk.Size);
		}

		public override string ToString () {
			return string.Format ("chunk ({0}, {1})", X, Z);
		}
	}
}