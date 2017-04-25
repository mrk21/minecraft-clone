using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class GrassBlock : BaseBlock {
		static private readonly GameObject Prefab = (GameObject) Resources.Load ("Prefabs/GrassBlock");

		public override GameObject GetPrefab() {
			return Prefab;
		}
	}
}