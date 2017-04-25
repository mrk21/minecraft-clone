using UnityEngine;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Application.Behaviour {
	class BlockBehavior : MonoBehaviour {
		public BaseBlock block;

		void OnDestroy() {
			block.RemoveFromTerrain ();
		}
	}
}