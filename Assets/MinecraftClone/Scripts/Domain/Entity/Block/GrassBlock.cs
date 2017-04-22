using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Domain.Entity.Block {
	class GrassBlock : BaseBlock {
		public GrassBlock(GameObject parent, Vector3 position) : base(parent, position) {
			obj.GetComponent<Renderer> ().material = Resources.Load ("Materials/GrassBlock", typeof(Material)) as Material;
		}
	}
}
