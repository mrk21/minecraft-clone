using UnityEngine;

namespace MinecraftClone.Domain.Block {
	class SandBlock : BaseBlock {
		public override void Draw(GameObject parent, Vector3 position) {
			base.Draw(parent, position);
			obj.GetComponent<Renderer> ().material = Resources.Load ("Materials/SandBlock", typeof(Material)) as Material;
		}
	}
}