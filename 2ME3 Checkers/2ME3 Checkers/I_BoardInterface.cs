using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2ME3_Checkers
{
    interface I_BoardInterface
    {

        void setUpBoard(string input);
        Piece getPiece(int column, int row); // check if piece exists and if true, return it
        Piece getPiece(Vector2 location); // piece can be searched for with a Vector2 as well
        void placePiece(int column, int row, Piece piece); // place piece on board
        void movePiece(int fromCol, int fromRow, int toCol, int toRow); // move piece
        void clear(); // removes all pieces from the board
    }
}
