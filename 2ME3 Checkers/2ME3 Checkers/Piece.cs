using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    class Piece : I_PieceInterface
    {
        //variable declarations
        private int column;
        private int row;
        public enum typeState { NORMAL, KING, NULL };
        private typeState pieceType;

        // CONSTRUCTORS

        public Piece(int column, int row, typeState pieceType)
        {
            this.column = column;
            this.row = row;
            this.pieceType = pieceType;
        }

        // METHODS

        //getters
        public typeState getType() { return pieceType; }
        public int getLocationX() { return this.column; }
        public int getLocationY() { return this.row; }
        
        public bool getValidMovement(int column, int row) 
        {
            //TODO Setup this function for assignment 2
            return false;
        }

        //setters
        public void setType(typeState newType) { this.pieceType = newType; } // king piece vs normal piece
        public void setLocation(int column, int row) { this.column = column; this.row = row; }
    }
}
