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
        /// This is a wrapper class for the sprites (buttons and pieces) so that we can test for
        ///     when we mouseclick on them by calling the Intersect() method when the mouse is pressed.
        /// Majority of the code modified from here http://xboxforums.create.msdn.com/forums/t/53705.aspx
        /// </summary>
        private Vector2 position;
        private Texture2D texture;
        private Vector2 size;
        private Color color;
        private float scale;
        private Vector2 boardCoords; // If the View_Clickable is a piece, the Piece information is stored
        private Piece pieceInfo;

        /// <summary>
        /// Constructor takes standard sprite.draw parameters.
        /// If for some reason we need more draw parameters, make another constructor; don't modify this one.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="scale"></param>
        public View_Clickable(Texture2D texture, Vector2 position, Color color, float scale)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.scale = scale;
            
            size = new Vector2(texture.Width*scale, texture.Height*scale);
            
        }

        public View_Clickable(Texture2D texture, Vector2 position, Color color, float scale, int col, int row)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.scale = scale;

            size = new Vector2(texture.Width * scale, texture.Height * scale);
            boardCoords = new Vector2(col, row);
        }

        public View_Clickable(Texture2D texture, Vector2 position, Color color, float scale, Piece piece)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.scale = scale;

            size = new Vector2(texture.Width * scale, texture.Height * scale);
            pieceInfo = piece;
        }

        /// <summary>
        /// When called, it checks if inputted xy screen coordinate lies within the sprite.
        /// </summary>
        /// <param name="mouseHit">Vector of the current X and Y coordinates of the mouse.</param>
        /// <returns>True if the coordinate is inside the sprite</returns>
        public bool IsIntersected(Vector2 mouseHit) 
        {
            Vector2 topLeft = position;
            Vector2 bottomRight = position + size;

            return (mouseHit.X >= topLeft.X && mouseHit.X <= bottomRight.X && mouseHit.Y >= topLeft.Y && mouseHit.Y <= bottomRight.Y);
        }

        /// <summary>
        /// Does the actual drawing to screen when called.
        /// </summary>
        /// <param name="batch">Pass the variable spriteBatch into this.</param>
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
        public Vector2 getCoords()
        {
            //if (pieceInfo != null)
                return boardCoords;
            //else
            //    throw new Exception("No piece to fetch from View_Clickable");
        }
        public Piece getPiece()
        {
            //if (pieceInfo != null)
            return pieceInfo;
            //else
            //    throw new Exception("No piece to fetch from View_Clickable");
        }
    }
}
