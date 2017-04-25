using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	class StoneBlock : BaseBlock {
		static private readonly GameObject Prefab = (GameObject) Resources.Load ("Prefabs/StoneBlock");

		public override GameObject GetPrefab() {
			return Prefab;
		}
	}
}