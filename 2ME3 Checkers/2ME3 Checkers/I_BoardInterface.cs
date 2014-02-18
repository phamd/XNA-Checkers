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
        bool getOccupied(int column, int row);
        string getOccupiedBy(int column, int row);

        //setters
        void setLocation(int column, int row, Piece newPiece);
        void clear();
    }
}
