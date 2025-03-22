using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Renderer;
using System.Threading.Tasks;

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

        public async Task DrawArroundChunk(Chunk chunk)
        {
            isDrawing.Value = true;
            int n = 5;

            for (int x = -n; x <= n; x++)
            {
                for (int z = -n; z <= n; z++)
                {
                    var baseAddress = chunk.Address;
                    var address = new ChunkAddress(baseAddress.X + x, baseAddress.Z + z);
                    await terrainRenderer.Draw(address);
                }
            }
            isDrawing.Value = false;
        }

        public async Task PutBlock(Ray ray, float range)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                Vector3 position = hit.point + 0.5f * hit.normal;

                if (dimension.Blocks[position].Traits.IsReplaceable())
                {
                    var block = GrassBlock.Create();
                    dimension.Blocks[position] = block;
                    await terrainRenderer.Redraw(position);
                }
            }
        }

        public async Task RemoveBlock(Ray ray, float range)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                Vector3 position = hit.point - 0.5f * hit.normal;

                if (dimension.Blocks[position].Traits.IsBreakable())
                {
                    dimension.Blocks[position] = AirBlock.Create();
                    await terrainRenderer.Redraw(position);
                    var factory = new FluidPropagatorFactory();
                    MainThreadDispatcher.Post((_) =>
                    {
                        StartCoroutine("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(dimension, position));
                    }, null);
                }
                else if (dimension.Blocks[position].Traits.IsReplaceable() && dimension.Blocks[position].Traits.IsBreakable())
                {
                    dimension.Blocks[position] = AirBlock.Create();
                    await terrainRenderer.Redraw(position);
                    var factory = new FluidPropagatorFactory();
                    MainThreadDispatcher.Post((_) =>
                    {
                        StartCoroutine("DrawFluid", factory.CreateFromRemovingAdjoiningBlock(dimension, position));
                    }, null);
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
