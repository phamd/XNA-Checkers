using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _2ME3_Checkers
{
    /// <summary>
    /// Currently, Erica:
    ///   * The three states work by keyboard hotkeys = { M, P, S }
    ///   * Going between P (playing) and M (menu) doesn't reset the board
    ///   * Pressing S (setup) will reset the board
    ///   
    /// TODO: 
    ///   * Add mouse control (added drag and drop mechanics)
    ///   * Distinguish between the two players (in Piece.cs/Board.cs and in the graphics)
    ///   * Board parser using E3=W, A4=B notation.
    ///   
    /// Thoughts:
    ///   * Maybe instead of tiling each square of the board, we just hardcode a single image in?
    /// </summary>
    
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        /// <summary>
        /// Variable declarations
        /// </summary>
        private enum STATE { MENU, SETUP, PLAYING };
        private STATE currentState = STATE.MENU;
        private KeyboardState keyState;
        private string input;

        private Board board = new Board();

        /// <summary>
        /// Textures/Graphics
        /// </summary>
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D Board_SquareWhite;
        private Texture2D Board_SquareBlack;
        private Texture2D Piece_BlackNormal;
        private Texture2D Piece_WhiteNormal;
        private Texture2D Piece_BlackKing;
        private Texture2D Piece_WhiteKing;
        private Texture2D Menu_ButtonPlay;
        private Texture2D Menu_ButtonCustom;
        // buttons
        private View_Clickable clickable_PlayButton;
        private View_Clickable clickable_CustomButton;

        private List<View_Clickable> pieceList = new List<View_Clickable>(); // list of pieces

        private bool piecesCreated = false;

        // we can hard code these following two variables
        private int board_SquareSize = 64; // pixel width to upscale to // keep it a multiple of the actual image
        private float board_squareScale;

        /// <summary>
        /// Mouse stuff
        /// </summary>
        private MouseState mouseStateCurrent;
        private MouseState mouseStatePrev;
        private View_Clickable mouseClickedPiece = null; // the current clicked on piece for dragging
        private Vector2 mousePos; // current mouse position
        private Vector2 mouseOffset; // offset from where mouse is dragged



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800; // resolution isn't a big deal, since we can scale the game to the viewport
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            Window.Title = "Checkers";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            /*for (int row = 7; row >= 0; row--)
            {
                for (int col = 0; col < 8; col++)
                {
                    
                    Console.Write(board.getOccupiedBy(col,row) + " ");
                    if (col == 7)
                        Console.WriteLine();
                }
            }*/

            this.IsMouseVisible = true;
            base.Initialize();
            Console.Title = "Checkers Console";
            Console.WriteLine("Welcome to Checkers");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);// SpriteBatch draw textures.

            Board_SquareBlack = this.Content.Load<Texture2D>("textures/Board_SquareBlack"); // make sure these two squares are the same pixel size
            Board_SquareWhite = this.Content.Load<Texture2D>("textures/Board_SquareWhite"); 
            board_squareScale = board_SquareSize / Board_SquareBlack.Height; // calculate the percent scaling we need to get the right square size
            Piece_BlackNormal = this.Content.Load<Texture2D>("textures/Piece_BlackNormal");
            Piece_WhiteNormal = this.Content.Load<Texture2D>("textures/Piece_WhiteNormal");
            Piece_BlackKing = this.Content.Load<Texture2D>("textures/Piece_BlackKing");
            Piece_WhiteKing = this.Content.Load<Texture2D>("textures/Piece_WhiteKing");
            Menu_ButtonPlay = this.Content.Load<Texture2D>("textures/Menu_ButtonPlay");
            Menu_ButtonCustom = this.Content.Load<Texture2D>("textures/Menu_ButtonCustom");
            
            base.LoadContent(); // not sure if this line should be at the end of the method

            // Buttons
            clickable_PlayButton = new View_Clickable(Menu_ButtonPlay, new Vector2(GraphicsDevice.Viewport.Width / 2 - Menu_ButtonPlay.Width / 2,
                GraphicsDevice.Viewport.Height * 1 / 3), Color.White, 1f);             // create play button
            clickable_CustomButton = new View_Clickable(Menu_ButtonCustom, new Vector2(GraphicsDevice.Viewport.Width / 2 - Menu_ButtonCustom.Width / 2,
                GraphicsDevice.Viewport.Height * 2 / 3), Color.White, 1f);            // create setup button
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            spriteBatch.Dispose();
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // Change state using the keyboard
            keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.M))
                currentState = STATE.MENU;
            else if (keyState.IsKeyDown(Keys.S))
                currentState = STATE.SETUP;
            else if (keyState.IsKeyDown(Keys.P))
                currentState = STATE.PLAYING;

            // Mouse Update Stuff
            mouseStatePrev = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();
            mousePos = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);

            // Drop piece if mouse button is up
            if (mouseStateCurrent.LeftButton == ButtonState.Released) mouseClickedPiece = null;

            // True if we pressed the mouse this frame.
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released)
            {
                // Look through all pieces
                foreach (View_Clickable thisPiece in pieceList)
                {
                    if (thisPiece.IsIntersected(mousePos)) // if we are clicking on the piece
                    {
                        mouseClickedPiece = thisPiece;
                        mouseOffset = thisPiece.getPosition() - mousePos;
                        break; // break so we can only pick up one piece at a time
                    }
                }

                // MENU buttons events // another option is to put all buttons in a buttonList and loop through
                if (currentState == STATE.MENU) {
                    if (clickable_CustomButton.IsIntersected(mousePos))
                            currentState = STATE.SETUP;
                    if (clickable_PlayButton.IsIntersected(mousePos))
                            currentState = STATE.PLAYING;
                }    

            }
            // If we haven't released the mouse, continue updating the position of the piece.
            if (mouseClickedPiece != null) mouseClickedPiece.setPosition(mousePos + mouseOffset);

            base.Update(gameTime);
        }


        /// <summary>
        /// The game draws different things depending on $currentState.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(); // drawing goes after this line

            if (currentState == STATE.MENU) 
            {
                // draw menu
                clickable_PlayButton.Draw(spriteBatch);
                clickable_CustomButton.Draw(spriteBatch);
            }

            else if (currentState == STATE.SETUP)
            {
                // draw setup
                // set up new board
                pieceList.Clear(); // clear the board first
                piecesCreated = false; // reset pieces
                
                if(input == null)
                    takeInput();
            }

            else if (currentState == STATE.PLAYING)
            {
                // draw board
                for (int row = 7; row >= 0; row--) // drawing from bottom up; (since 0,0 is top left corner graphically)
                {
                    for (int col = 0; col < 8; col++) // drawing from left to right first
                    {
                        Texture2D tile;
                        if ((row % 2 == 0 && col % 2 == 0) || (row % 2 != 0 && col % 2 != 0)) // alternating between black and white; tiled pattern
                        {
                            tile = Board_SquareWhite;
                        }
                        else
                        {
                            tile = Board_SquareBlack;
                        }

                        // draw tiles
                        spriteBatch.Draw(tile, new Vector2(32 + board_SquareSize * col, 32 + board_SquareSize * row), // the 32 is the gap to the left and top of the board
                                null, Color.White, 0f, new Vector2(0, 0), board_squareScale, SpriteEffects.None, 0);

                        // check if a piece belongs on the tile then draws it.
                        if (piecesCreated == false) { // we only want to _create_ the pieces once // If we want to reset the board, set this to false
                            if (board.getPiece(col, 7 - row) != null) // looks at the board array
                            {
                                // we wrap each piece in a class called View_Clickable so we can apply additional methods to them
                                Texture2D pieceTexture;
                                if (board.getPiece(col, 7 - row).getOwner() == Piece.player.BLACK) {
                                    switch (board.getPiece(col, 7 - row).getType())
                                    {
                                        case (Piece.typeState.NORMAL):
                                            pieceTexture = Piece_BlackNormal;
                                            break;
                                        case (Piece.typeState.KING):
                                            pieceTexture = Piece_BlackKing;
                                            break;
                                        default:
                                            throw new Exception("No way");
                                    }
                                }
                                else if (board.getPiece(col, 7 - row).getOwner() == Piece.player.WHITE)
                                {
                                    switch (board.getPiece(col, 7 - row).getType())
                                    {
                                        case (Piece.typeState.NORMAL):
                                            pieceTexture = Piece_WhiteNormal;
                                            break;
                                        case (Piece.typeState.KING):
                                            pieceTexture = Piece_WhiteKing;
                                            break;
                                        default:
                                            throw new Exception("No way");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error: Piece owned by someone not BLACK or WHITE!");
                                }

                                // prepare a list of all pieces so we can draw them later
                                pieceList.Add(new View_Clickable(pieceTexture, new Vector2(32 + board_SquareSize * col + board_SquareSize / 2 - Piece_BlackNormal.Width / 2,
                                    32 + board_SquareSize * row + board_SquareSize / 2 - Piece_BlackNormal.Height / 2), Color.White, 1f)); // 32 offsets again, maybe put these into a variable
                            }
                        }
                    }
                }

                // draw pieces
                foreach (View_Clickable sprite in pieceList) // we tell each piece to draw // actually, we can do this right after adding them to the list
                {
                    sprite.Draw(spriteBatch);
                }

                piecesCreated = true; // We set true to tell the PLAYING state to not create brand new copies of our pieces every frame
                input = null; // We reset the SETUP state so we can set up an new board
            } // END OF PLAYING STATE

            spriteBatch.End(); // drawing goes before this line
            base.Draw(gameTime);
        }
        /// <summary>
        /// This function will ask the user for a string to parse as the board setup
        /// There are no arguments
        /// The input string will be null before entering the function
        /// </summary>
        void takeInput()
        {
            input = "setup board"; // if the input is not null the function will not be called every frame
            Console.WriteLine("Input a board in the format of A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B");
            Console.WriteLine("No more that 12 of each colour, and only place on solid squares");
            input = Console.ReadLine();
            //input = "A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B"; // sample input
            try
            {
                board = new Board(input); // this construction is able to throw exceptions 
                // the following code only runs if there were no exceptions above
                Console.WriteLine("Setting up board: " + input);
                Console.WriteLine("Enjoy your game");
                currentState = STATE.PLAYING;
            }
            catch
            {
                Console.WriteLine("Please format the input correctly");
                Console.WriteLine();
                takeInput();
            }
        }
    }
}
