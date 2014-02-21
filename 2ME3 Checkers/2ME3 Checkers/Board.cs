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
                        pieceArray[col, row] = new Piece(col, row, Piece.typeState.NORMAL, Piece.player.BLACK);
                    else if ((col % 2 != 0 && (row == 5 || row == 7)) || (col % 2 == 0 && row == 6)) // top player's area
                        pieceArray[col, row] = new Piece(col, row, Piece.typeState.NORMAL, Piece.player.WHITE);
                    else
                        pieceArray[col, row] = new Piece(col, row, Piece.typeState.NULL, Piece.player.NULL);
                }
            }
        }

        public Board(String input)
        {
            // sample input string: "A1=W, C1=W, E1=W, G1=W, A7=B, B8=B"
            // parse this into the pieceArray[,]
            //  * split1 on commas, split2 again on equal sign, split3 left side again to 1 character
            //  * coord.x = split3[0], coord.y = split3[1], owner = split2[1]


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
                    pieceArray[col, row] = new Piece(col, row, Piece.typeState.NULL, Piece.player.NULL);
                }
            }
        }
    }
}
