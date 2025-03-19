using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Block;
using System.Threading.Tasks;

namespace MinecraftClone.Domain.Renderer
{
    class ChunkMesh
    {
        public Mesh renderer;
        public Mesh collider;
    }

    class ChunkMeshFactory
    {
        private static readonly int OpaqueSubMeshIndex = 0;
        private static readonly int TransparentSubMeshIndex = 1;

        private Chunk chunk;

        public ChunkMeshFactory(Chunk chunk)
        {
            this.chunk = chunk;
        }

        public async Task<ChunkMesh> Create()
        {
            var builderForRenderer = new BlockMeshBuilder(subMeshCount: 2);
            var builderForCollider = new BlockMeshBuilder(subMeshCount: 1);

            var textureFactory = new BlockTextureFactory();

            await Task.Run(() => {
                for (int y = 0; y < Chunk.Depth; y++)
                {
                    for (int x = 0; x < Chunk.Size; x++)
                    {
                        for (int z = 0; z < Chunk.Size; z++)
                        {
                            var block = chunk[x, y, z];
                            if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Void) continue;

                            var position = new Vector3(x, y, z);
                            var texture = textureFactory.Create(block);

                            if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Solid)
                            {
                                builderForCollider.AddBlockMesh(position, texture, 0);

                                BuildSolidMesh(builderForCollider, x, y, z);
                            }

                            if (block.Traits.IsTransparent())
                            {
                                builderForRenderer.AddBlockMesh(position, texture, TransparentSubMeshIndex);

                                if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Solid)
                                {
                                    BuildSolidMesh(builderForRenderer, x, y, z);
                                }
                                else if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Fluid)
                                {
                                    BuildFluidMesh(builderForRenderer, x, y, z);
                                }
                            }
                            else
                            {
                                builderForRenderer.AddBlockMesh(position, texture, OpaqueSubMeshIndex);

                                if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Solid)
                                {
                                    BuildSolidMesh(builderForRenderer, x, y, z);
                                }
                                else if (block.Traits.MatterType == BlockTraits.MatterTypeEnum.Fluid)
                                {
                                    BuildFluidMesh(builderForRenderer, x, y, z);
                                }
                            }
                        }
                    }
                }
            });
            var meshFilter = chunk.GameObjects["Chunk"].GetComponent<MeshFilter>();
            var meshCollider = chunk.GameObjects["Chunk"].GetComponent<MeshCollider>();

            var mesh = new ChunkMesh();
            mesh.renderer = builderForRenderer.ToMesh(meshFilter.sharedMesh);
            mesh.collider = builderForCollider.ToMesh(meshCollider.sharedMesh);
            return mesh;
        }

        private void BuildSolidMesh(BlockMeshBuilder builder, int x, int y, int z)
        {
            System.Func<BaseBlock, bool> isValidBlock = (block) =>
            {
                return block.Traits.IsTransparent();
            };
            if ((y == Chunk.Depth - 1) || (y + 1 <  Chunk.Depth && isValidBlock(chunk[x,     y + 1, z    ]))) builder.AddXZ2Plane();
            if (                          (y - 1 >= 0           && isValidBlock(chunk[x,     y - 1, z    ]))) builder.AddXZPlane();
            if ((z == Chunk.Size - 1)  || (z + 1 <  Chunk.Size  && isValidBlock(chunk[x,     y,     z + 1]))) builder.AddXY2Plane();
            if ((z == 0)               || (z - 1 >= 0           && isValidBlock(chunk[x,     y,     z - 1]))) builder.AddXYPlane();
            if ((x == Chunk.Size - 1)  || (x + 1 <  Chunk.Size  && isValidBlock(chunk[x + 1, y,     z    ]))) builder.AddYZ2Plane();
            if ((x == 0)               || (x - 1 >= 0           && isValidBlock(chunk[x - 1, y,     z    ]))) builder.AddYZPlane();
        }

        private void BuildFluidMesh(BlockMeshBuilder builder, int x, int y, int z)
        {
            System.Func<BaseBlock, bool> isValidBlock = (block) =>
            {
                return block.Traits.MatterType != BlockTraits.MatterTypeEnum.Fluid
                    && block.Traits.IsTransparent();
            };
            if ((y == Chunk.Depth - 1) || (y + 1 < Chunk.Depth && isValidBlock(chunk[x    , y + 1, z    ]))) builder.AddXZ2Plane();
            if (                          (y - 1 >= 0          && isValidBlock(chunk[x    , y - 1, z    ]))) builder.AddXZPlane();
            if (                          (z + 1 < Chunk.Size  && isValidBlock(chunk[x    , y    , z + 1]))) builder.AddXY2Plane();
            if (                          (z - 1 >= 0          && isValidBlock(chunk[x    , y    , z - 1]))) builder.AddXYPlane();
            if (                          (x + 1 < Chunk.Size  && isValidBlock(chunk[x + 1, y    , z    ]))) builder.AddYZ2Plane();
            if (                          (x - 1 >= 0          && isValidBlock(chunk[x - 1, y    , z    ]))) builder.AddYZPlane();
        }
    }
}