using UnityEngine;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Application.Behaviour {
	class BlockBehavior : MonoBehaviour {
		public BaseBlock block;

		public void Remove() {
			block.RemoveFromTerrain ();
			Destroy (gameObject);
		}
	}
}