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
            for (int i = 0; i < 8; i++)
            {
                for (int g = 0; g < 8; g++)
                {
                    if ((i%2 == 0 && (g == 0 || g == 2)) || (i%2 != 0 && g == 1))
                        pieceArray[i, g] = new Piece(i, g, "1");
                    else if ((i % 2 == 0 && (g == 5 || g == 7)) || (i % 2 != 0 && g == 6))
                        pieceArray[i, g] = new Piece(i, g, "2");
                    else
                        pieceArray[i, g] = new Piece(i, g, "_");
                }
            }
        }

        //getters
        public bool getOccupied(int column, int row) { return pieceArray[column, row].getType() != "NULL"; }
        public string getOccupiedBy(int column, int row) { return pieceArray[column, row].getType(); }
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
            for (int i = 0; i < 8; i++)
            {
                for (int g = 0; g < 8; g++)
                {
                    pieceArray[i, g] = new Piece(i, g, "NULL");
                }
            }
        }
    }
}
