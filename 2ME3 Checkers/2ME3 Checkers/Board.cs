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
            // sample input string: "A1=W, C1=W, E1=W, G1=WK, A7=B, B8=B"
            // parse this into the pieceArray[,]

            clear();

            string[] splitCommas = input.Split(','); // need to remove whitespace too
            string[] splitEquals;
            int coordCol;
            int coordRow;
            Piece.player player;
            Piece.typeState type;
            for (int i = 0; i < splitCommas.Length; i++)
            {
                splitEquals = splitCommas[i].Split('=');
                splitEquals[0].Substring(0, 1);
                coordRow = Convert.ToInt16(splitEquals[0].Substring(1, 1)); // need to add a check to see if it's actually an int in range 1-8
                // internally 0-7 instead of 1-8 so we subtract 1
                coordRow -= 1;

                switch (splitEquals[0].Substring(0,1).ToUpper()) { // associate the column names with its index
                    case("A"):
                        coordCol = 0;
                        break;
                    case("B"):
                        coordCol = 1;
                        break;
                    case ("C"):
                        coordCol = 2;
                        break;
                    case ("D"):
                        coordCol = 3;
                        break;
                    case ("E"):
                        coordCol = 4;
                        break;
                    case ("F"):
                        coordCol = 5;
                        break;
                    case ("G"):
                        coordCol = 6;
                        break;
                    case ("H"):
                        coordCol = 7;
                        break;
                    default:
                        coordCol = -99; // make this throw an error
                        break;
                }

                switch (splitEquals[1])
                {
                    case("B"):
                        player = Piece.player.BLACK;
                        type = Piece.typeState.NORMAL;
                        break;
                    case("W"):
                        player = Piece.player.WHITE;
                        type = Piece.typeState.NORMAL;
                        break;
                    case ("BK"):
                        player = Piece.player.BLACK;
                        type = Piece.typeState.KING;
                        break;
                    case ("WK"):
                        player = Piece.player.WHITE;
                        type = Piece.typeState.KING;
                        break;
                    default:
                        player = Piece.player.NULL;
                        type = Piece.typeState.NULL;
                        break;
                }

                pieceArray[coordCol, coordRow] = new Piece(coordCol, coordRow, type, player);


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
                    pieceArray[col, row] = new Piece(col, row, Piece.typeState.NULL, Piece.player.NULL);
                }
            }
        }
    }
}
