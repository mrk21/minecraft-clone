using System;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Infrastructure;
using MinecraftClone.Application.Behaviour;

namespace MinecraftClone.Domain.Block {
	abstract class BaseBlock : IEntity<int> {
		protected int id;
		protected int blockId;
		protected BlockTraits traits;

		public event Action<BaseBlock> OnRemoveFromTerrain;

		public BaseBlock() {
			this.id = GetHashCode();
			this.blockId = 0;
		}

		public int BlockId {
			get { return blockId; }
		}

		public int Id {
			get { return id; }
		}

		public BlockTraits Traits {
			get { return traits; }
		}

		public void RemoveFromTerrain() {
			if (OnRemoveFromTerrain != null) OnRemoveFromTerrain (this);
		}

		public override string ToString () {
			return string.Format ("{0}#{1}", GetType().FullName, id);
		}
	}
}
