using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class SandBlock : BaseBlock {
		static private readonly GameObject Prefab = (GameObject) Resources.Load ("Prefabs/SandBlock");

		public override GameObject GetPrefab() {
			return Prefab;
		}
	}
}