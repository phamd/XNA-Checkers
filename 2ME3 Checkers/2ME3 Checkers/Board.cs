using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    class Board //: I_BoardInterface
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
                        pieceArray[col, row] = new Piece(Piece.typeState.NORMAL, Piece.player.BLACK);
                    else if ((col % 2 != 0 && (row == 5 || row == 7)) || (col % 2 == 0 && row == 6)) // top player's area
                        pieceArray[col, row] = new Piece(Piece.typeState.NORMAL, Piece.player.WHITE);
                    else
                        pieceArray[col, row] = null;
                }
            }
        }

        /// <summary>
        /// Custom setup of the board
        /// <param name="input"> the input string which will be interpreted as the piece locations </param>
        /// Throws an exceptions on all malformed inputs. This whole constructor will be within a try catch statement in the main file. 
        /// If the constructor has an exception, the main file will know the input is invalid
        /// </summary>
        public Board (String input)
        {
            // sample input string: "A1=W, C1=W, E1=W, G1=WK, A7=B, B8=B"
            // parse this into the pieceArray[,]

            clear();

            string[] splitCommas = input.Split(','); // splits "A1=W,C1=W" on the comma
            string[] splitEquals;
            int coordCol;
            int coordRow;
            int numWhitePieces = 0;
            int numBlackPieces = 0;
            Piece.player player;
            Piece.typeState type;
            for (int i = 0; i < splitCommas.Length; i++)
            {
                splitEquals = splitCommas[i].Split('='); // split "A1=W" on the equals sign
                
                if (splitEquals[0].Length != 2) throw new Exception(); // if left side is not "A1", but "A12" or "AA1" then error
                coordRow = Convert.ToInt16(splitEquals[0].Substring(1, 1)); // convert the row number an int
                // internally 0-7 instead of 1-8 so we subtract 1
                coordRow -= 1;

                switch (splitEquals[0].Substring(0,1).ToUpper()) { // associate the column names with its index
                    case ("A"):
                        coordCol = 0;
                        break;
                    case ("B"):
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
                        //if the input isn't recognized, then throw an exception
                        throw new Exception();
                }

                switch (splitEquals[1].ToUpper()) // the right side of the equal sign in A1=W
                {
                    case ("B"):
                        player = Piece.player.BLACK;
                        type = Piece.typeState.NORMAL;
                        numBlackPieces++;
                        break;
                    case ("W"):
                        player = Piece.player.WHITE;
                        type = Piece.typeState.NORMAL;
                        numWhitePieces++;
                        break;
                    case ("BK"):
                        player = Piece.player.BLACK;
                        type = Piece.typeState.KING;
                        numBlackPieces++;
                        break;
                    case ("WK"):
                        player = Piece.player.WHITE;
                        type = Piece.typeState.KING;
                        numWhitePieces++;
                        break;
                    default:
                        //if the input isn't recognized, then throw an exception
                        throw new Exception();
                }

                //Too many pieces check
                if(numBlackPieces > 12 || numWhitePieces > 12) {
                    Console.Write("You can only have up to 12 of one kind of piece and ");
                    if (numWhitePieces > numBlackPieces)
                        Console.WriteLine("you had " + numWhitePieces + " white pieces");
                    else
                        Console.WriteLine("you had " + numBlackPieces + " black pieces");
                    throw new Exception();
                }
                
                //Invalid placement check
                if (coordCol % 2 != coordRow % 2)
                {
                    Console.WriteLine("Invalid placement. Only place on solid board squares");
                    throw new Exception();
                }
                pieceArray[coordCol, coordRow] = new Piece(type, player);
            }

        }

        public Piece getPiece(int column, int row) { return pieceArray[column, row]; } // returns null for no piece

        public void movePiece(int fromCol, int fromRow, int toCol, int toRow)
        {
            // put piece into new location
            this.pieceArray[toCol, toRow] = this.pieceArray[fromCol, fromRow];
            // remove piece from previous location
            this.pieceArray[fromCol, fromRow] = null;
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
                    pieceArray[col, row] = null;
                }
            }
        }
    }
}
