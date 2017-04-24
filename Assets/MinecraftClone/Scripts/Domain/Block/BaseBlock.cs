using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block {
	abstract class BaseBlock : IEntity<int> {
		protected GameObject obj;
		protected int id;

		public BaseBlock() {
			this.id = GetHashCode();
		}

		public int Id {
			get { return id; }
		}

		public virtual void Draw(GameObject parent, Vector3 position) {
			obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			obj.transform.parent = parent.transform;
			obj.name = string.Format ("{0}#{1}({2},{3},{4})",
				GetType().FullName.Replace("MinecraftClone.Domain.Block.", ""),
				id,
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

		public override string ToString () {
			return string.Format ("{0}#{1}", GetType().FullName, id);
		}
	}
}
