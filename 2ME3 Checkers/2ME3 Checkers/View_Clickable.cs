using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _2ME3_Checkers
{

    
    class View_Clickable
    {

        /// <summary>
        /// This is a wrapper class for the pieces sprites so that we can test for 
        /// intersection with the mouse for drag and drop.
        /// Majority of the code modified from here http://xboxforums.create.msdn.com/forums/t/53705.aspx
        /// Feel free to refactor the class name View_Pieces to something else. ~ Don
        /// </summary>
        private Vector2 position;
        private Texture2D texture;
        private Vector2 size;
        private Color color;
        private float scale;

        // constructor  
        public View_Clickable(Texture2D texture, Vector2 position, Color color, float scale)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.scale = scale;

            size = new Vector2(texture.Width, texture.Height);
        }

        public bool Intersect(Vector2 mouseHit)
        {
            Vector2 topLeft = position;
            Vector2 bottomRight = position + size;

            return (mouseHit.X >= topLeft.X && mouseHit.X <= bottomRight.X && mouseHit.Y >= topLeft.Y && mouseHit.Y <= bottomRight.Y);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, position, null, color, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }
        public Vector2 getPosition()
        {
            return this.position;
        }
        public void setPosition(Vector2 pos)
        {
            this.position = pos;
        }
    }
}
