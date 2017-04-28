using System;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Domain.Block {
	abstract class BaseBlock : IEntity<int> {
		protected int id;

		public event Action<BaseBlock> OnRemoveFromTerrain;

		public BaseBlock() {
			this.id = GetHashCode();
		}

		public virtual int BlockId {
			get { return 0; }
		}

		public int Id {
			get { return id; }
		}

		public virtual bool IsTransparent {
			get { return false; }
		}

		public virtual bool IsVoid {
			get { return false; }
		}

		public void RemoveFromTerrain() {
			OnRemoveFromTerrain (this);
		}

		public override string ToString () {
			return string.Format ("{0}#{1}", GetType().FullName, id);
		}
	}
}
