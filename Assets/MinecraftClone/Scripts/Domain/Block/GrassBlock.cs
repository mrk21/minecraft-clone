using UnityEngine;

namespace MinecraftClone.Domain.Block {
	class GrassBlock : BaseBlock {
		public override void Draw(GameObject parent, Vector3 position) {
			base.Draw(parent, position);
			obj.GetComponent<Renderer> ().material = Resources.Load ("Materials/GrassBlock", typeof(Material)) as Material;
		}
	}
}