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
        public void setValidMovementsTest()
        {
            using (Game1 game = new Game1())
            {
                Board board = new Board();
                board.setUpBoard("A1=W"); // White piece in bottom left corner
                String[] expectedResult = { "invalid", "1,1", "invalid", "invalid" };
                String[] actualResult = new String[4];

                game.setValidMovements(board, 0, 0);
                for (int i = 0; i < 4; i++ )
                {
                    actualResult[i] = board.getPiece(0, 0).getValidMovements()[i].ToString();
                    if (actualResult[i].Split(',')[0] == "-99") actualResult[i] = "invalid";
                    Console.WriteLine(expectedResult[i] + " : " + actualResult[i]);
                    Assert.AreEqual(expectedResult[i], actualResult[i]);
                }
                
            }
        }
    }
}
