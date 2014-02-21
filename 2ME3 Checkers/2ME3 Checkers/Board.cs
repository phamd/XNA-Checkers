using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    class Board : I_BoardInterface
    {
        //variables
        private Piece[,] pieceArray = new Piece[8,8];

        //Constructors

        /// <summary>
        /// Default setup of the board
        /// </summary>
        public Board()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if ((col % 2 == 0 && (row == 0 || row == 2)) || (col % 2 != 0 && row == 1)) // bottom player's area
                        pieceArray[col, row] = new Piece(col, row, Piece.typeState.NORMAL);
                    else if ((col % 2 != 0 && (row == 5 || row == 7)) || (col % 2 == 0 && row == 6)) // top player's area
                        pieceArray[col, row] = new Piece(col, row, Piece.typeState.NORMAL);
                    else
                        pieceArray[col, row] = new Piece(col, row, Piece.typeState.NULL);
                }
            }
        }

        //getters
        public bool getOccupied(int column, int row) { return pieceArray[column, row].getType() != Piece.typeState.NULL; }
        public Piece.typeState getOccupiedBy(int column, int row) 
        {
            if (pieceArray[column, row] != null)
            {
                return pieceArray[column, row].getType();
            }
            else
            {
                return Piece.typeState.NULL;
            }
        }
        public Piece getPiece(int column, int row) { return pieceArray[column, row]; }

        //setters
        public void setLocation(int column, int row, Piece newPiece)
        {
            this.pieceArray[column, row] = newPiece;   
        }

        /// <summary>
        /// Clears the board
        /// </summary>
        public void clear()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    pieceArray[col, row] = new Piece(col, row, Piece.typeState.NULL);
                }
            }
        }
    }
}
