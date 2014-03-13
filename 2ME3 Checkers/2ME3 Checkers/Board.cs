using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2ME3_Checkers
{
    class Board : I_BoardInterface
    {
        //variables
        private Piece[,] pieceArray = new Piece[8,8];
        private int numWhitePieces = 0;
        private int numBlackPieces = 0;
        //Constructors

        /// <summary>
        /// Default setup of the board
        /// </summary>
        public Board()
        {
            this.clear(); // clear the board first

            // default setup
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if ((col % 2 == 0 && (row == 0 || row == 2)) || (col % 2 != 0 && row == 1)) // bottom player's area
                        placePiece(col, row, new Piece(Piece.typeState.NORMAL, Piece.player.BLACK));
                    else if ((col % 2 != 0 && (row == 5 || row == 7)) || (col % 2 == 0 && row == 6)) // top player's area
                        placePiece(col, row, new Piece(Piece.typeState.NORMAL, Piece.player.WHITE));
                    else
                    {
                        // place nothing
                    }
                        
                }
            }
        }

        /// <summary>
        /// Custom setup of the board
        /// <param name="input"> the input string which will be interpreted as the piece locations </param>
        /// Throws an exceptions on all malformed inputs. This whole constructor will be within a try catch statement in the main file. 
        /// If the constructor has an exception, the main file will know the input is invalid
        /// </summary>
        public void setUpBoard (String input)
        {
            // sample input string: "A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B"
            // the goal is to parse this string and place the pieces on the board
            numBlackPieces = 0;
            numWhitePieces = 0;
            clear(); // we assume the user will setup the board all at once in one line
                     // we can later give the option to allow the user to set up the board one piece at a time.

            string[] splitCommas = input.Split(','); // splits "A1=W,C1=W" on the comma
            string[] splitEquals;
            int coordCol;
            int coordRow;

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
 
                placePiece(coordCol, coordRow, new Piece(type, player)); // place the piece with the parsed information
            }

        }

        /// <summary>
        /// This method is used to determine if a piece exists on a spot (returns null for no).
        /// If the piece does exist, we pass it along to the caller so it can do specific Piece methods such as getType() or getOwner().
        /// </summary>
        public Piece getPiece(int column, int row)
        {
            try { return pieceArray[column, row]; }
            catch { throw new Exception("Error: Trying to fetch piece outside of array"); }
        }

        public Piece getPiece(Vector2 location)
        {
            try { return pieceArray[(int) location.X, (int) location.Y]; }
            catch { throw new Exception("Error: Trying to fetch piece outside of array"); }
        }

        public Piece[,] getPieceArray()
        {
            return pieceArray;
        }

        /// <summary>
        /// Checks if a piece placement is legal, if so, it will place the piece there.
        /// </summary>
        public void placePiece(int column, int row, Piece piece) 
        {
            // Too many pieces check
            // Another safer option is to recount the pieceArray every time we add a piece instead of having a running count.
            if (numBlackPieces > 12 || numWhitePieces > 12)
            {
                Console.Write("You can only have up to 12 of one kind of piece and ");
                if (numWhitePieces > numBlackPieces)
                    Console.WriteLine("you had " + numWhitePieces + " white pieces");
                else
                    Console.WriteLine("you had " + numBlackPieces + " black pieces");
                throw new Exception();
            }

            // Invalid placement check
            if (column % 2 != row % 2)
            {
                Console.WriteLine("Invalid placement. Only place on solid board squares");
                throw new Exception();
            }

            pieceArray[column, row] = piece;
        }

        /// <summary>
        /// Checks if the movement of a piece is legal, if so, then we move the piece to that location.
        /// </summary>
        public void movePiece(int fromCol, int fromRow, int toCol, int toRow)
        {
            // check if movement is legal

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
