﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Renderer;

namespace MinecraftClone.Application.WorldScene
{
    class TerrainView : MonoBehaviour
    {
        [SerializeField] private GameObject chunkPrefab = null;
        public ReactiveProperty<bool> isDrawing;
        private TerrainRenderer terrainRenderer;
        private Dimension dimension;
        
        void Start()
        {
            isDrawing = new ReactiveProperty<bool>(false);
        }

        public void Init(Dimension dimension)
        {
            this.dimension = dimension;
            terrainRenderer = new TerrainRenderer(dimension, gameObject, chunkPrefab);
            terrainRenderer.Init();
        }

        public void DrawArroundChunk(Chunk chunk)
        {
            isDrawing.Value = true;

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    var baseAddress = chunk.Address;
                    var address = new ChunkAddress(baseAddress.X + x, baseAddress.Z + z);
                    terrainRenderer.Draw(address);
                }
            }
            isDrawing.Value = false;
        }

        public void PutBlock(Ray ray, float range)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                Vector3 position = hit.point + 0.5f * hit.normal;

                if (dimension.Blocks[position].Traits.IsReplaceable())
                {
                    var block = new GrassBlock();
                    dimension.Blocks[position] = block;
                    terrainRenderer.Redraw(position);
                }
            }
        }

        public void RemoveBlock(Ray ray, float range)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                Vector3 position = hit.point - 0.5f * hit.normal;

                if (dimension.Blocks[position].Traits.IsBreakable())
                {
                    dimension.Blocks[position].RemoveFromTerrain();
                    terrainRenderer.Redraw(position);
                    var factory = new FluidPropagatorFactory();
                    StartCoroutine("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(dimension, position));
                }
                else if (dimension.Blocks[position].Traits.IsReplaceable() && dimension.Blocks[position].Traits.IsBreakable())
                {
                    dimension.Blocks[position].RemoveFromTerrain();
                    terrainRenderer.Redraw(position);
                    var factory = new FluidPropagatorFactory();
                    StartCoroutine("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(dimension, position));
                }
            }
        }

        private IEnumerator DrawFluid(FluidPropagator propagator)
        {
            yield return new WaitForSeconds(0.5f);

            foreach (var items in propagator.Start())
            {
                var chunkAddresses = new HashSet<ChunkAddress>();
                foreach (var item in items)
                {
                    dimension.Blocks[item.Position] = item.Block;
                    chunkAddresses.Add(ChunkAddress.FromPosition(item.Position));
                }
                foreach (var address in chunkAddresses)
                {
                    terrainRenderer.Redraw(address);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
