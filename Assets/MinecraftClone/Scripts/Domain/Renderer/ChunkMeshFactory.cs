using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Map;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Domain.Renderer {
	class ChunkMesh {
		public Mesh collider;
		public Mesh opaque;
		public Mesh transparent;
	}

	class ChunkMeshFactory {
		private Chunk chunk;

		public ChunkMeshFactory(Chunk chunk) {
			this.chunk = chunk;
		}

		public ChunkMesh Create() {
			var transparentBuilder = new BlockMeshBuilder ();
			var opaqueBuilder = new BlockMeshBuilder ();
			var colliderBuilder = new BlockMeshBuilder ();

			var textureFactory = new BlockTextureFactory ();

			for (int y=0; y < Chunk.Depth; y++) {
				for (int x=0; x < Chunk.Size; x++) {
					for (int z=0; z < Chunk.Size; z++) {
						var block = chunk [x, y, z];
						if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Void) continue;

						var position = new Vector3 (x, y, z);
						var texture = textureFactory.Create (block);

						if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Solid) {
							colliderBuilder.AddBlockMesh (position, texture);

							BuildSolidMesh (colliderBuilder, x, y, z);
						}

						if (block.Traits.IsTransparent ()) {
							transparentBuilder.AddBlockMesh (position, texture);

							if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Solid) {
								BuildSolidMesh (transparentBuilder, x, y, z);
							} else if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Fluid) {
								BuildFluidMesh (transparentBuilder, x, y, z);
							}
						} else {
							opaqueBuilder.AddBlockMesh (position, texture);

							if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Solid) {
								BuildSolidMesh (opaqueBuilder, x, y, z);
							} else if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Fluid) {
								BuildFluidMesh (opaqueBuilder, x, y, z);
							}
						}
					}
				}
			}

			var mesh = new ChunkMesh ();
			mesh.collider = colliderBuilder.ToMesh ();
			mesh.opaque = opaqueBuilder.ToMesh ();
			mesh.transparent = transparentBuilder.ToMesh ();
			return mesh;
		}

		private void BuildSolidMesh(BlockMeshBuilder builder, int x, int y, int z) {
			System.Func<BaseBlock, bool> isValidBlock = (block) => {
				return block.Traits.IsTransparent();
			};
			if ((y == Chunk.Depth - 1) || (y + 1 <    Chunk.Depth && isValidBlock (chunk [x, y + 1, z]))) builder.AddXZ2Plane ();
			if (                          (y - 1 >= 0             && isValidBlock (chunk [x, y - 1, z]))) builder.AddXZPlane ();
			if ((z == Chunk.Size - 1)  || (z + 1 <    Chunk.Size  && isValidBlock (chunk [x, y, z + 1]))) builder.AddXY2Plane ();
			if ((z == 0             )  || (z - 1 >= 0             && isValidBlock (chunk [x, y, z - 1]))) builder.AddXYPlane ();
			if ((x == Chunk.Size - 1)  || (x + 1 <    Chunk.Size  && isValidBlock (chunk [x + 1, y, z]))) builder.AddYZ2Plane ();
			if ((x == 0             )  || (x - 1 >= 0             && isValidBlock (chunk [x - 1, y, z]))) builder.AddYZPlane ();
		}

		private void BuildFluidMesh(BlockMeshBuilder builder, int x, int y, int z) {
			System.Func<BaseBlock, bool> isValidBlock = (block) => {
				return block.Traits.MatterType != BlockTraits.MatterTypeEnum.Fluid
					&& block.Traits.IsTransparent();
			};
			if ((y == Chunk.Depth - 1) || (y + 1 <    Chunk.Depth && isValidBlock (chunk [x, y + 1, z]))) builder.AddXZ2Plane ();
			if (                          (y - 1 >= 0             && isValidBlock (chunk [x, y - 1, z]))) builder.AddXZPlane ();
			if (                          (z + 1 <    Chunk.Size  && isValidBlock (chunk [x, y, z + 1]))) builder.AddXY2Plane ();
			if (                          (z - 1 >= 0             && isValidBlock (chunk [x, y, z - 1]))) builder.AddXYPlane ();
			if (                          (x + 1 <    Chunk.Size  && isValidBlock (chunk [x + 1, y, z]))) builder.AddYZ2Plane ();
			if (                          (x - 1 >= 0             && isValidBlock (chunk [x - 1, y, z]))) builder.AddYZPlane ();
		}
	}
}