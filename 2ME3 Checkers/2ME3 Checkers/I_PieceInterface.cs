using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    interface I_PieceInterface
    {
        
        //getters
        Piece.typeState getType(); // can't define enums in interfaces, so we put it in Pieces.cs //http://stackoverflow.com/questions/15009073/interfaces-cannot-declare-types
        public Piece.player getOwner();
        int getLocationX();
        int getLocationY();
        bool getValidMovement(int column, int row); // input a location and check if it is valid to move there

        //setters
        void setType(Piece.typeState newType); // king piece vs normal piece
        void setLocation(int column, int row);
    }
}
