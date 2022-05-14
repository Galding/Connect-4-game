using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace puissance4.DesktopClient
{
    internal class Puissance4Object
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _size;

        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public Vector2 Size { get { return _size; } set { _size = value; } }

        public Puissance4Object(Texture2D texture, Vector2 position, Vector2 size)
        {
            _texture = texture;
            _position = position;
            _size = size;
        }
    }
}
