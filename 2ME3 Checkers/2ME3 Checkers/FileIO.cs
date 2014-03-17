using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace _2ME3_Checkers
{
    class FileIO:I_FileIOInterface
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/CheckersGameSaves"; //designate a path for the folder

        public static void save(Board board)
        {
            string saveText = "";
            int jAdjusted = 0;  //adding one to index j for board index

            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Folder already exists");
                }
                else
                {
                    DirectoryInfo directory = Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Save Unsuccessful " + e);
            }

            for (int i = 0; i < pieceArray.Length / 8; i++)
            {
                for (int j = 0; j < pieceArray.Length / 8; j++)
                {
                    if (board.getPiece(i, j) != null)
                    {
                        //Saving Row
                        switch (i)
                        {
                            case (0):
                                saveText += "A";
                                break;
                            case (1):
                                saveText += "B";
                                break;
                            case (2):
                                saveText += "C";
                                break;
                            case (3):
                                saveText += "D";
                                break;
                            case (4):
                                saveText += "E";
                                break;
                            case (5):
                                saveText += "F";
                                break;
                            case (6):
                                saveText += "G";
                                break;
                            case (7):
                                saveText += "H";
                                break;
                        }

                        //Saving Column
                        jAdjusted = j + 1;
                        saveText += jAdjusted.ToString() + "=";

                        //Saving Colour
                        if (board.getPiece(i, j).getOwner() == Piece.player.WHITE)
                        {
                            saveText += "W";
                        }
                        else
                        {
                            saveText += "B";
                        }

                        //Saving Piece Type
                        if (board.getPiece(i, j).getType() == Piece.typeState.KING)
                        {
                            saveText += "K";
                        }

                        saveText += ",";
                    }
                }
            }
            saveText = saveText.Substring(0, saveText.Length - 1); //Get rid of trailing comma
            System.IO.File.WriteAllText(@path + "/SavedGame.txt", saveText); //write to file
            Console.WriteLine("Game Saved!");
        }

        public static string load(Board board)
        {
            if (File.Exists(@path + "/SavedGame.txt"))
            {
                return System.IO.File.ReadAllText(@path + "/SavedGame.txt");
            }
            else
            {
                Console.WriteLine("Save File Does Not Exist!");
                return "no save file";
            }
        }
    }
}