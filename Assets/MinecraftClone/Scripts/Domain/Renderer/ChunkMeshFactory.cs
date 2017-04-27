using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Domain.Renderer {
	class ChunkMeshFactory {
		private Chunk chunk;

		public ChunkMeshFactory(Chunk chunk) {
			this.chunk = chunk;
		}

		public Mesh Create() {
			var builder = new BlockMeshBuilder ();
			var textureFactory = new BlockTextureFactory ();

			for (int y=0; y < Chunk.Depth; y++) {
				for (int x=0; x < Chunk.Size; x++) {
					for (int z=0; z < Chunk.Size; z++) {
						var block = chunk [x, y, z];
						if (block == null) continue;

						var position = new Vector3 (x, y, z);
						var texture = textureFactory.Create (block);
						builder.AddBlockMesh (position, texture);
						 
						if ((y >= Chunk.Size - 1) || (y + 1 <    Chunk.Depth && chunk [x, y + 1, z] == null)) builder.AddXZ2Plane ();
						if (                         (y - 1 >= 0             && chunk [x, y - 1, z] == null)) builder.AddXZPlane ();
						if ((z == Chunk.Size - 1) || (z + 1 <    Chunk.Size  && chunk [x, y, z + 1] == null)) builder.AddXY2Plane ();
						if ((z == 0             ) || (z - 1 >= 0             && chunk [x, y, z - 1] == null)) builder.AddXYPlane ();
						if ((x == Chunk.Size - 1) || (x + 1 <    Chunk.Size  && chunk [x + 1, y, z] == null)) builder.AddYZ2Plane ();
						if ((x == 0             ) || (x - 1 >= 0             && chunk [x - 1, y, z] == null)) builder.AddYZPlane ();
					}
				}
			}
			return builder.ToMesh ();
		}
	}
}