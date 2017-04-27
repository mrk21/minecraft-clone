using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Domain.Renderer {
	class TextureMap {
		private int size;

		public TextureMap(int size) { this.size = size; }

		public int Size { get { return size; } }
		public float ItemScale { get { return 1f / size; } }

		public TextureItem GetItem(Vector2 offset) {
			return new TextureItem (offset, this);
		}
	}
}