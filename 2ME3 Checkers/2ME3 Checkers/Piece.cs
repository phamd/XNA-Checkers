using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    public class Piece : I_PieceInterface
    {
        //variable declarations
        public enum TYPESTATE { NORMAL, KING }; // enums are public so the other classes can know the allowed values.
        private TYPESTATE pieceType;
        public enum PLAYER { BLACK, WHITE };
        private PLAYER owner;

        //This struct contains all the info needed for a valid move location
        public struct validMovementsStruct {
            public validMoveDirection direction; 
            public int col; 
            public int row;

            public validMovementsStruct(validMoveDirection direction, int col, int row)
            {
                this.direction = direction; 
                this.col = col; 
                this.row = row;
            }
            public override String ToString()
            {
                return this.col + "," + this.row;
            }
        };
        //list of all the valid movements
        private validMovementsStruct[] validMovementsArray = new validMovementsStruct[4];

        // The 4 ways a piece can move
        public enum validMoveDirection { UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT }  //Currently unused //Can refactor the setValidMovements function to take this as a param
        

        // CONSTRUCTORS

        public Piece(TYPESTATE pieceType, PLAYER owner)
        {
            this.pieceType = pieceType;
            this.owner = owner;
        }

        public Piece()
        {
        }

        // METHODS

        //getters
        public TYPESTATE getType() { return pieceType; }
        public PLAYER getOwner() { return this.owner; }
        public validMovementsStruct[] getValidMovements()
        {
            return validMovementsArray;
        }
       

        //setters
        public void setType(TYPESTATE newType) { this.pieceType = newType; } // king piece vs normal piece

        //Sets the places that is valid for the piece to move to
        public void setValidMovements(validMoveDirection direction, int col, int row)
        {
            //directions 0,1,2,3 are upleft, upright,downright,downleft
            //this.validMovements[direction,0] = col;
            if (direction == validMoveDirection.UP_LEFT)
                validMovementsArray[0] = new validMovementsStruct(direction, col, row);
            else if (direction == validMoveDirection.UP_RIGHT)
                validMovementsArray[1] = new validMovementsStruct(direction, col, row);
            else if (direction == validMoveDirection.DOWN_RIGHT)
                validMovementsArray[2] = new validMovementsStruct(direction, col, row);
            else if (direction == validMoveDirection.DOWN_LEFT)
                validMovementsArray[3] = new validMovementsStruct(direction, col, row);
            else
                throw new Exception("Error. Direction not handled"); 
        }

    }
}
