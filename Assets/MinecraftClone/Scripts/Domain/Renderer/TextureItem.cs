using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Domain.Renderer
{
    class TextureItem
    {
        private TextureMap map;
        private Vector2 offset;

        public TextureItem(Vector2 offset, TextureMap map)
        {
            this.offset = offset;
            this.map = map;
        }
        public Vector2 Offset { get { return offset; } }
        public float Scale { get { return map.ItemScale; } }
        public TextureMap Map { get { return map; } }
    }
}