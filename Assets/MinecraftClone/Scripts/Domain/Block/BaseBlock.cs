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

		public virtual GameObject GetPrefab() {
			return null;
		}

		public void Draw(GameObject parent, Vector3 position) {
			obj = GameObject.Instantiate (GetPrefab (), position, Quaternion.identity, parent.transform);
			var behavior = obj.GetComponent<BlockBehaviour> ();
			behavior.block = this;
		}

		public void RemoveFromTerrain() {
			OnRemoveFromTerrain (this);
		}

		public void Unload() {
			GameObject.Destroy(obj);
		}

		public override string ToString () {
			return string.Format ("{0}#{1}", GetType().FullName, id);
		}
	}
}
