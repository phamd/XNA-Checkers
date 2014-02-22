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
    /// Currently:
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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Variable declarations
        /// </summary>
        private enum STATE { MENU, SETUP, PLAYING };
        private STATE currentState = STATE.MENU;
        private KeyboardState keyState;

        private Board board = new Board();

        /// <summary>
        /// Textures/Graphics
        /// </summary>
        Texture2D Board_SquareWhite; // square == tile
        Texture2D Board_SquareBlack;
        Texture2D Piece_BlackNormal;
        Texture2D Piece_WhiteNormal;
        Texture2D Menu_ButtonPlay;

        List<View_Pieces> pieceList = new List<View_Pieces>(); // list of pieces
        bool piecesDrawn = false;
        private int board_SquareSize = 64; // pixel width to upscale to // keep it a multiple of the actual image
        private float board_squareScale;

        /// <summary>
        /// Mouse stuff
        /// </summary>
        private MouseState mouseStateCurrent;
        private MouseState mouseStatePrev;
        View_Pieces mouseClickedPiece = null; // the current clicked on piece for dragging
        Vector2 mouseOffset; // Offset from where mouse is dragged

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
            // TODO: Add your initialization logic here

            for (int row = 7; row >= 0; row--)
            {
                for (int col = 0; col < 8; col++)
                {
                    
                    Console.Write(board.getOccupiedBy(col,row) + " ");
                    if (col == 7)
                        Console.WriteLine();
                }
            }

            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content 
            Board_SquareBlack = this.Content.Load<Texture2D>("textures/Board_SquareBlack"); // make sure these two squares are the same pixel size
            Board_SquareWhite = this.Content.Load<Texture2D>("textures/Board_SquareWhite"); 
            board_squareScale = board_SquareSize / Board_SquareBlack.Height; // calculate the percent scaling we need to get the right square size
            Piece_BlackNormal = this.Content.Load<Texture2D>("textures/Piece_BlackNormal");
            Piece_WhiteNormal = this.Content.Load<Texture2D>("textures/Piece_WhiteNormal");
            Menu_ButtonPlay = this.Content.Load<Texture2D>("textures/Menu_ButtonPlay");
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
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
            // TODO: Add your update logic here
            
            //Console.WriteLine(currentState.ToString());
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
                spriteBatch.Draw(Menu_ButtonPlay, new Vector2(GraphicsDevice.Viewport.Width / 2 - Menu_ButtonPlay.Width / 2,
                    GraphicsDevice.Viewport.Height * 1/3), Color.White);
            }

            else if (currentState == STATE.SETUP)
            {
                // draw setup
                // set up new board, then switch to STATE.PLAYING to use it
                //board = new Board("A1=W, C1=W, E1=W, G1=W, A7=B, B8=B");
                pieceList.Clear(); // clear the board first
                piecesDrawn = false; // reset pieces

                // the console spams if we use it here
                //Console.WriteLine("Input a board in the format of A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B");
                //string input = Console.ReadLine();

                string input = "A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B"; // sample input

                if (input != null)
                {
                    board = new Board(input);
                }

            }

            else if (currentState == STATE.PLAYING)
            {
                // draw board
                for (int row = 7; row >= 0; row--) // drawing from bottom up; (since 0,0 is top left corner)
                {
                    for (int col = 0; col < 8; col++) // drawing from left to right 
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

                        // create pieces
                        if (piecesDrawn == false) { // we only want to _create_ the pieces once // If we want to reset the board, set this to false
                            if (board.getOccupiedBy(col, 7 - row) == Piece.typeState.NORMAL) // looks at the board array
                            {
                                // we wrap each piece in a class called View_Pieces so we can add the intersect function
                                Texture2D pieceTexture;
                                if (board.getPiece(col, 7 - row).getOwner() == Piece.player.BLACK) {
                                    pieceTexture = Piece_BlackNormal;
                                }    
                                else
                                {
                                    pieceTexture = Piece_WhiteNormal;
                                }
                                
                                pieceList.Add(new View_Pieces(pieceTexture, new Vector2(32 + board_SquareSize * col + board_SquareSize / 2 - Piece_BlackNormal.Width / 2,
                                    32 + board_SquareSize * row + board_SquareSize / 2 - Piece_BlackNormal.Height / 2), Color.White, 1f)); // 32 offsets again, maybe put these into a variable
                            }
                        }
                    }
                }


                // draw pieces
                foreach (View_Pieces sprite in pieceList) // we tell each piece to draw // actually, we can do this in the loop up there, too
                {
                    sprite.Draw(spriteBatch);
                }

                piecesDrawn = true;
            } // END OF PLAYING STATE

            ///////////////////////////////////////////////////////http://xboxforums.create.msdn.com/forums/t/53705.aspx
            // Mouse Update Stuff
            mouseStatePrev = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();

            Vector2 mouseHit = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);

            // If the mouse button is up, drop whatever we have (if anything).  
            if (mouseStateCurrent.LeftButton == ButtonState.Released) mouseClickedPiece = null;
            // Was the mouse button pressed this frame?  
            bool mouseDown = mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released;

            if (mouseDown)
            {
                // Test each MouseSprite  
                foreach (View_Pieces sprite in pieceList)
                {
                    if (sprite.Intersect(mouseHit))
                    {
                        // We have a hit.  
                        mouseClickedPiece = sprite;
                        mouseOffset = sprite.position - mouseHit;
                        break;
                    }
                }
            }
            // If dragging, update the position of the sprite.  
            if (mouseClickedPiece != null)
            {
                mouseClickedPiece.position = mouseHit + mouseOffset;
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            spriteBatch.End(); // drawing goes before this line
            base.Draw(gameTime);
        }
    }
}
