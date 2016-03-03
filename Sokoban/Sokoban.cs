using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    public enum Direction { RIGHT = 0, LEFT, DOWN, UP }

    class Sokoban
    {
        Texture2D image;
        Vector2 position;
        Direction direction;
        int spriteWidth, spriteHeight;

        public Sokoban(ContentManager content, Vector2 position)
        {
            this.position = position;
            this.direction = Direction.RIGHT;
            image = content.Load<Texture2D>("character");

            spriteWidth = image.Width / 3;
            spriteHeight = image.Height / 4;
        } 

        public void Move(Vector2 movement)
        {
            if (movement.X == 1)
                direction = Direction.RIGHT;
            else if (movement.X == -1)
                direction = Direction.LEFT;
            else if (movement.Y == 1)
                direction = Direction.DOWN;
            else
                direction = Direction.UP;

            position = position + movement;  
        }

        public Vector2 Position()
        {
            return position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                image,
                position: position * 64,
                color: Color.White,
                sourceRectangle: new Rectangle(0,
                     spriteHeight * (int)direction,
                     spriteWidth, spriteHeight)
            );
        }

    }
}
