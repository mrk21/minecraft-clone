using UnityEngine;

namespace MinecraftClone.Domain.Map {
	struct ChunkAddress {
		public int x;
		public int z;

		public ChunkAddress(int x, int z) {
			this.x = x;
			this.z = z;
		}

		public static ChunkAddress FromPosition(Vector3 position) {
			return new ChunkAddress (
				Mathf.FloorToInt (1f * position.x / Chunk.Size),
				Mathf.FloorToInt (1f * position.z / Chunk.Size)
			);
		}

		public Vector3 ToPosition() {
			return new Vector3 (x * Chunk.Size, 0f, z * Chunk.Size);
		}

		public override string ToString () {
			return string.Format ("chunk ({0}, {1})", x, z);
		}
	}
}