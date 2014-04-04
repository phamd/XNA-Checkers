using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    interface I_PieceInterface
    {
        
        //getters
        Piece.TYPESTATE getType(); // can't define enums in interfaces, so we put it in Pieces.cs
        Piece.PLAYER getOwner();
        Piece.validMovementsStruct[] getValidMovements(); // returns the list of valid movement structs
        bool canJump();

        //setters
        void setType(Piece.TYPESTATE newType); // king piece vs normal piece
        void setValidMovements(Piece.validMoveDirection direction, int col, int row, bool isAjump); 

    }
}
