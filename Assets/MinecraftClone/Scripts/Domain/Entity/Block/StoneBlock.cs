using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Domain.Entity.Block {
	class StoneBlock : BaseBlock {
		public StoneBlock(GameObject parent, Vector3 position) : base(parent, position) {
			obj.GetComponent<Renderer> ().material = Resources.Load ("Materials/StoneBlock", typeof(Material)) as Material;
		}
	}
}
