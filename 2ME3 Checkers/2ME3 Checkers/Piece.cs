using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    class Piece : I_PieceInterface
    {
        //variable declarations
        public enum typeState { NORMAL, KING }; // enums are public so the other classes can know the allowed values.
        private typeState pieceType;

        public enum player { BLACK, WHITE }; //probably shouldn't have a null player, but it simplifies things for now
        private player owner;

        // CONSTRUCTORS

        public Piece(typeState pieceType, player owner)
        {
            this.pieceType = pieceType;
            this.owner = owner;
        }

        public Piece()
        {
        }

        // METHODS

        //getters
        public typeState getType() { return pieceType; }
        public player getOwner() { return this.owner; }
        //TEMP
        public string test() { return "hello world"; }

        //setters
        public void setType(typeState newType) { this.pieceType = newType; } // king piece vs normal piece
    }
}
