using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Domain.Entity.Block {
	abstract class BaseBlock {
		public static readonly int BlockSize = 1;
		public GameObject obj;

		public BaseBlock(GameObject parent, Vector3 position) {
			obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			obj.transform.parent = parent.transform;
			obj.name = string.Format ("Block({0},{1},{2})",
				position.x,
				position.y,
				position.z
			);
			obj.transform.position = new Vector3 (
				position.x * BlockSize,
				position.y * BlockSize,
				position.z * BlockSize
			);
			obj.transform.localScale = new Vector3 (
				BlockSize,
				BlockSize,
				BlockSize
			);
		}
	}
}
