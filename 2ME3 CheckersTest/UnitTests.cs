using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2ME3_Checkers;

namespace _2ME3_CheckersTest
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void BoardTestPass()
        {
            try
            {
                Board board = new Board();
                board.setUpBoard("A1=W,A3=W,A7=B,B2=W,B6=B,B8=B,C1=W,C3=W,C7=B,D2=W,D6=B,D8=B,E1=W,E3=W,E7=B,F2=W,F6=B,F8=B,G1=W,G3=W,G7=B,H2=W,H6=B,H8=B");
                board.setUpBoard("A1=W,A3=W,A7=B,B2=W,B6=B,B8=B,C1=W,C5=W,D8=B,E1=BK,E5=W,E7=B,F6=W,F8=B,G1=W,G7=B,H2=W,H6=B,H8=B");
                board.setUpBoard("a1=wk,A3=bk,A7=bK,B2=Wk,B6=B,B8=B,C1=W,C5=W,D8=B,E1=Bk,e5=W,e7=B,F6=W,F8=B,g1=W,G7=B,H2=W,H6=B,h8=B");
            }
            catch
            {
                Assert.Fail(); // No input should reach this.
            }
        }
        [TestMethod]
        public void BoardTestFail()
        { // This test will return successfully if all of the inputs fail.
            Board board = new Board();
            String[] badSetups = { "\n", ",,", "A1=W ,", "A1=FFFF", "A1=FFX", "Aq=vvcxW", "A2=W,B2=BK", "A1=KKKK",
                                 "A1=W,A3=W,A7=B,B2=W,B6=B,B8=B,C1=W,C5=W,D8=B,E1=BK,E5=W,E8=B,F6=W,F8=B,G1=W,G7=B,H2=W,H6=B,H8=B"};
            foreach (String input in badSetups)
            {
                bool didFail = false;
                try
                {
                    board.setUpBoard(input);
                }
                catch (Exception e)
                {
                    didFail = true;
                }
                finally
                {
                    Assert.IsTrue(didFail); // Each input should fail.
                }
            }
        }
        [TestMethod]
        public void setValidMovementsTestSimpleCase()
        {
            using (Game1 game = new Game1())
            {
                Board board = new Board();
                board.setUpBoard("A1=W"); // White piece in bottom left corner
                String[] expectedResult = { "illegal", "1,1", "illegal", "illegal" };
                String[] actualResult = new String[4];

                game.setValidMovements(board, 0, 0);
                for (int i = 0; i < 4; i++ )
                {
                    actualResult[i] = board.getPiece(0, 0).getValidMovements()[i].ToString();
                    if (actualResult[i].Split(',')[0] == "-99") actualResult[i] = "illegal";
                    Console.WriteLine(expectedResult[i] + " : " + actualResult[i]);
                    Assert.AreEqual(expectedResult[i], actualResult[i]);
                }
                
            }
        }

        [TestMethod]
        public void BoardLegalMovesTest_DefaultSetup()
        {
            // The default new game board
            String boardSetup = "A1=W,A3=W,A7=B,B2=W,B6=B,B8=B,C1=W,C3=W,C7=B,D2=W,D6=B,D8=B,E1=W,E3=W,E7=B,F2=W,F6=B,F8=B,G1=W,G3=W,G7=B,H2=W,H6=B,H8=B";
            // The legal moves of each piece (four possible diagonal directions). Null indicates an empty space on the board.
            String[][] expectedValidMoves = new String[64][] { new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { "illegal", "1,3", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "2,4", "0,4" }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { "1,3", "3,3", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "4,4", "2,4" }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { "3,3", "5,3", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "6,4", "4,4" }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { "5,3", "7,3", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "6,4" }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" } };
            BoardLegalMovesTest(boardSetup, expectedValidMoves);
        }

        [TestMethod]
        public void BoardLegalMovesTest_MidgameSetup()
        {
            // A setup of the middle of a checkers match with multiple kings.
            String boardSetup = "A1=W,A3=W,A7=B,B4=B,B8=B,C1=BK,C5=B,C7=W,D4=BK,D6=W,E3=W,F4=B,F6=B,F8=WK,G1=W,G3=W,G5=W,G7=WK,H6=B";
            String[][] expectedValidMoves = new String[64][] { new String[] { "illegal", "1,1", "illegal", "illegal" }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "1,5", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "2,2", "0,1" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { "1,1", "3,1", "3,-99", "1,-99" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { "0,7", "3,7", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "4,4", "5,1", "2,2" }, new String[] { null }, new String[] { "illegal", "4,6", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "7,1", "3,1" }, new String[] { null }, new String[] { "illegal", "illegal", "7,3", "4,4" }, new String[] { null }, new String[] { "4,-99", "6,-99", "illegal", "4,6" }, new String[] { "5,1", "7,1", "illegal", "illegal" }, new String[] { null }, new String[] { "4,4", "7,3", "illegal", "illegal" }, new String[] { null }, new String[] { "4,6", "7,6", "illegal", "illegal" }, new String[] { null }, new String[] { "illegal", "7,7", "7,4", "4,4" }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { null }, new String[] { "illegal", "illegal", "illegal", "illegal" }, new String[] { null }, new String[] { null } };
            BoardLegalMovesTest(boardSetup, expectedValidMoves);
        }

        public void BoardLegalMovesTest(String boardSetup, String[][] expectedValidMoves)
        {
            using (Game1 game = new Game1())
            {
                Board board = new Board();
                board.setUpBoard(boardSetup);
                game.setValidMovements(board);

                // Expected movements
                String[][] expectedResult = expectedValidMoves;
                String[] actualResult = new String[4]; // four possible directions to check per piece

                // For each piece on the board
                for (int col = 0; col < 8; col++)
                {
                    for (int row = 0; row < 8; row++)
                    {
                        try
                        {
                            Piece piece = board.getPiece(col, row);
                            Console.WriteLine(" -- (" + col + ", " + row + ") -- ");
                            for (int i = 0; i < 4; i++)
                            {
                                actualResult[i] = board.getPiece(col, row).getValidMovements()[i].ToString();
                                if (actualResult[i].Split(',')[0] == "-99") actualResult[i] = "illegal";
                                Console.WriteLine(expectedResult[col * 8 + row][i].ToString() + " : " + actualResult[i]);
                                // Compare actual valid movement to expected valid movement for each direction.
                                Assert.AreEqual(expectedResult[col * 8 + row][i].ToString(), actualResult[i]);
                            }
                        }
                        catch
                        {
                            // If piece doesn't exist, then we expect it to be null.
                            Assert.IsTrue(expectedResult[col * 8 + row][0] == null);
                        }
                    }
                }
            }
        }
    }
}
