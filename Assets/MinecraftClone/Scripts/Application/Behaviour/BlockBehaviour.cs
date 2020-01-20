using UnityEngine;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Application.Behaviour
{
    class BlockBehaviour : MonoBehaviour
    {
        public BaseBlock block = null;

        public void Remove()
        {
            block.RemoveFromTerrain();
            Destroy(gameObject);
        }
    }
}