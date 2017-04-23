using UnityEngine;

namespace MinecraftClone.Domain.Block {
	abstract class BaseBlock {
		public GameObject obj;

		public virtual void Draw(GameObject parent, Vector3 position) {
			obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			obj.transform.parent = parent.transform;
			obj.name = string.Format ("Block({0},{1},{2})",
				position.x,
				position.y,
				position.z
			);
			obj.transform.position = new Vector3 (
				position.x,
				position.y,
				position.z
			);
		}
	}
}