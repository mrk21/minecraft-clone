using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Renderer {
	class BlockTextureFactory {
		private TextureMap map;

		public BlockTextureFactory() {
			this.map = new TextureMap(size: 5);
		}

		public TextureItem Create(BaseBlock block) {
			var x = block.BlockId % map.Size;
			var y = map.Size - block.BlockId / map.Size - 1;
			return map.GetItem (offset: new Vector2 (x, y));
		}
	}
}