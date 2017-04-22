using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Domain.Entity.Block {
	class SandBlock : BaseBlock {
		public SandBlock(GameObject parent, Vector3 position) : base(parent, position) {
			obj.GetComponent<Renderer> ().material = Resources.Load ("Materials/SandBlock", typeof(Material)) as Material;
		}
	}
}
