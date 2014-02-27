using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    interface I_BoardInterface
    {
        //getters
        Piece getPiece(int column, int row);
        bool isOccupied(int column, int row);
        Piece.typeState getOccupiedBy(int column, int row); // Piece.typeState is an enum of possible states

        //setters
        void setLocation(int column, int row, Piece newPiece);
        void clear(); // removes all pieces from the board
    }
}
