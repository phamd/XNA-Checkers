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
            public bool isAJump;

            public validMovementsStruct(validMoveDirection direction, int col, int row, bool isAJump)
            {
                this.direction = direction; 
                this.col = col; 
                this.row = row;
                this.isAJump = isAJump;
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
        public bool canJump()
        {
            //if any of the directions is a jump return true
            for (int i = 0; i < 4; i++)
               if(validMovementsArray[i].isAJump) return true;
            return false;
        }

        //setters
        public void setType(TYPESTATE newType) { this.pieceType = newType; } // king piece vs normal piece

        //Sets the places that is valid for the piece to move to
        public void setValidMovements(validMoveDirection direction, int col, int row, bool isAJump)
        {
            //directions 0,1,2,3 are upleft, upright,downright,downleft
            //this.validMovements[direction,0] = col;
            if (direction == validMoveDirection.UP_LEFT)
                validMovementsArray[0] = new validMovementsStruct(direction, col, row, isAJump);
            else if (direction == validMoveDirection.UP_RIGHT)
                validMovementsArray[1] = new validMovementsStruct(direction, col, row, isAJump);
            else if (direction == validMoveDirection.DOWN_RIGHT)
                validMovementsArray[2] = new validMovementsStruct(direction, col, row, isAJump);
            else if (direction == validMoveDirection.DOWN_LEFT)
                validMovementsArray[3] = new validMovementsStruct(direction, col, row, isAJump);
            else
                throw new Exception("Error. Direction not handled"); 
        }

        /*public static bool operator ==(Piece x, Piece y)
        {

            bool returnValue = true;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    if ((x.getValidMovements()[i].col != y.getValidMovements()[i].col) || (x.getValidMovements()[i].row != y.getValidMovements()[i].row))
                    {
                        returnValue = false;
                        break;
                    }
                }
                catch { return false; }
            }
            return returnValue;
        }
        public static bool operator !=(Piece x, Piece y)
        {
 
            bool returnValue = true;
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((x.getValidMovements()[i].col == y.getValidMovements()[i].col) || (x.getValidMovements()[i].row == y.getValidMovements()[i].row))
                    {
                        returnValue = false;
                    }
                }
            }
            catch { return false; }
            return returnValue;
        }*/
    }
}
