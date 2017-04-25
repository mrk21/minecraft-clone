using System;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Domain.Block {
	abstract class BaseBlock : IEntity<int> {
		protected GameObject obj;
		protected int id;

		public event Action<BaseBlock> OnRemoveFromTerrain;

		public BaseBlock() {
			this.id = GetHashCode();
		}

		public int Id {
			get { return id; }
		}

		public Vector3 Position {
			get { return obj.transform.position; }
		}

		public virtual void Draw(GameObject parent, Vector3 position) {
			obj = GameObject.CreatePrimitive (PrimitiveType.Cube);

			var behavior = obj.AddComponent<BlockBehavior> ();
			behavior.block = this;

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

		public void RemoveFromTerrain() {
			OnRemoveFromTerrain (this);
		}

		public override string ToString () {
			return string.Format ("{0}#{1}", GetType().FullName, id);
		}
	}
}
