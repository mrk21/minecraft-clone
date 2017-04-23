using UnityEngine;

namespace MinecraftClone.Domain.Block {
	class StoneBlock : BaseBlock {
		public override void Draw(GameObject parent, Vector3 position) {
			base.Draw(parent, position);
			obj.GetComponent<Renderer> ().material = Resources.Load ("Materials/StoneBlock", typeof(Material)) as Material;
		}
	}
}