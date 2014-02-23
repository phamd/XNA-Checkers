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

    
    class View_Pieces
    {

        /// <summary>
        /// This is a wrapper class for the pieces sprites so that we can test for 
        /// intersection with the mouse for drag and drop.
        /// Majority of the code modified from here http://xboxforums.create.msdn.com/forums/t/53705.aspx
        /// Feel free to refactor the class name View_Pieces to something else. ~ Don
        /// </summary>
        public Vector2 position; // this should be private 
        private Texture2D texture;
        private Vector2 size;
        private Color color;
        private float scale;
        private bool draw; // whether or not the sprite should get drawn in the playing state
        private string buttonName;

        // constructor  
        public View_Pieces(Texture2D texture, Vector2 position, Color color, float scale, bool draw = true, string buttonName = "")
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.scale = scale;
            this.draw = draw;
            this.buttonName = buttonName;

            size = new Vector2(texture.Width, texture.Height);
        }

        public bool Intersect(Vector2 mouseHit)
        {
            Vector2 min = position;
            Vector2 max = position + size;

            bool result = false;

            if (mouseHit.X >= min.X && mouseHit.X <= max.X
                && mouseHit.Y >= min.Y && mouseHit.Y <= max.Y)
            {
                result = true;
            }

            return result;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, position, null, color, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }

        public bool getDrawable()
        {
            return this.draw;
        }

        public void setDrawable(bool draw)
        {
            this.draw = draw;
        }

        public string getButtonName()
        {
            return this.buttonName;
        }
    }
}
