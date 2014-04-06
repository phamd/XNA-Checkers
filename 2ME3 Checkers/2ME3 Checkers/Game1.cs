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
    /// 2ME3 Checkers
    /// General todo: black and white are spawning on the wrong side of the board
    /// </summary>
    
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Variable declarations
        /// </summary>
        private enum STATE { MENU, SETUP, PLAYING, LOAD }; // The three major states of the game
        private STATE currentState = STATE.MENU; // The variable to track which state is currently active
        private Piece.PLAYER currentPlayerTurn = Piece.PLAYER.WHITE; // Start with WHITE going first
        private Piece.PLAYER lastPlayerTurn = Piece.PLAYER.BLACK; // The previous turn. this is saved because sometimes the same player can go multiple times
        private KeyboardState keyState;
        private string input;
        private FileIO fileIO = new FileIO();

        
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
        private Texture2D Menu_ButtonLoad;
        private Texture2D Playing_ButtonSave;
        private Texture2D Playing_ButtonMenu;

        // buttons
        private View_Clickable clickable_PlayButton;
        private View_Clickable clickable_CustomButton;
        private View_Clickable clickable_MenuButton;
        private View_Clickable clickable_LoadButton;
        private View_Clickable clickable_SaveButton;

        private List<View_Clickable> pieceList = new List<View_Clickable>(); // list of pieces

        private bool piecesCreated = false;

        // we can hard code these following two variables
        private const int board_SquareSize = 64; // pixel width to upscale to // keep it a multiple of the actual image
        private float board_squareScale;

        /// <summary>
        /// Mouse stuff
        /// </summary>
        private MouseState mouseStateCurrent;
        private MouseState mouseStatePrev;
        private View_Clickable mouseClickedPiece = null; // the current clicked on piece for dragging
        private Vector2 mousePos; // current mouse position
        private Vector2 mouseOffset; // offset from where mouse is dragged
        private Vector2 mouseBoardPosition; // the board index the mouse is hovering over

        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 600; // resolution isn't a big deal, since we can scale the game to the viewport
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
            Menu_ButtonLoad = this.Content.Load<Texture2D>("textures/Menu_ButtonLoad");
            Playing_ButtonMenu = this.Content.Load<Texture2D>("textures/Playing_ButtonMenu");
            Playing_ButtonSave = this.Content.Load<Texture2D>("textures/Playing_ButtonSave");
            
            // Buttons
            clickable_PlayButton = new View_Clickable(Menu_ButtonPlay, new Vector2(GraphicsDevice.Viewport.Width / 2 - Menu_ButtonPlay.Width / 2,
                GraphicsDevice.Viewport.Height * 1 / 9), Color.White, 1f);             // create play button
            clickable_CustomButton = new View_Clickable(Menu_ButtonCustom, new Vector2(GraphicsDevice.Viewport.Width / 2 - Menu_ButtonCustom.Width / 2,
                GraphicsDevice.Viewport.Height * 3 / 9), Color.White, 1f);            // create setup button
            clickable_LoadButton = new View_Clickable(Menu_ButtonLoad, new Vector2(GraphicsDevice.Viewport.Width / 2 - Menu_ButtonLoad.Width / 2,
                GraphicsDevice.Viewport.Height * 5 / 9), Color.White, 1f); //create load button
            clickable_MenuButton = new View_Clickable(Playing_ButtonMenu, new Vector2(GraphicsDevice.Viewport.Width - Menu_ButtonCustom.Width*0.3f,
                GraphicsDevice.Viewport.Height - Menu_ButtonCustom.Height*0.3f), Color.White, 0.3f);            // create setup button
            clickable_SaveButton = new View_Clickable(Playing_ButtonSave, new Vector2(GraphicsDevice.Viewport.Width - Menu_ButtonCustom.Width,
                GraphicsDevice.Viewport.Height - Menu_ButtonCustom.Height * 0.3f), Color.White, 0.3f); //create save button

            base.LoadContent();
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

            if (keyState.IsKeyDown(Keys.B))
                currentPlayerTurn = Piece.PLAYER.BLACK;
            else if (keyState.IsKeyDown(Keys.W))
                currentPlayerTurn = Piece.PLAYER.WHITE;

            //TEMP
            else if (keyState.IsKeyDown(Keys.K))
            {
                Console.WriteLine(board.getNumPieces(Piece.PLAYER.BLACK));
                Console.WriteLine(currentPlayerTurn);
                Console.WriteLine("jump available: " + board.getJumpAvailable()[0] + " " + board.getJumpAvailable()[1] + " jump piece set: " + (board.getJumpingPiece() != null));                                                                                                                                                                                                                                                                                                                                                                                                                                                
                
            }

            // Mouse Update Stuff
            mouseStatePrev = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();
            mousePos = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
            mouseBoardPosition = new Vector2((float) Math.Round( (mousePos.X - board_SquareSize) / board_SquareSize) 
                , (float) Math.Round(Math.Abs(mousePos.Y / (board_SquareSize) - 8))); // this is where the mouse is looking on the board ie (0-7), (0-7)
            
            // Drop piece if mouse button is up
            if ((mouseStateCurrent.LeftButton == ButtonState.Released && mouseClickedPiece != null))
            {
                //update the last person to have a turn
                lastPlayerTurn = currentPlayerTurn;
                // there are 4 possible directions of movement
                for(int i = 0; i < 4; i++) {
                    //need a this conditional because board.getPiece() can return null if there is no Piece there
                    if(board.getPiece(mouseClickedPiece.getCoords()) != null)
                    {
                        //if the place where the mouse is releasing the piece is a valid move for the piece 
                        if ( (board.getPiece(mouseClickedPiece.getCoords()).getValidMovements()[i].col == mouseBoardPosition.X)
                            && (board.getPiece(mouseClickedPiece.getCoords()).getValidMovements()[i].row == mouseBoardPosition.Y) )
                        {
                            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            ///Case 1: Jump Available && Jump Piece Set
                            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            //if there is a jump available and the jumping piece is set check if that piece can continue jumping and jump with it, else end the turn
                            if ( ((board.getJumpAvailable(Piece.PLAYER.WHITE) && currentPlayerTurn == Piece.PLAYER.WHITE)
                                || (board.getJumpAvailable(Piece.PLAYER.BLACK) && currentPlayerTurn == Piece.PLAYER.BLACK)) 
                                && board.getJumpingPiece() != null)
                            {
                                Console.WriteLine(currentPlayerTurn+" Case 1: Jump Available && Jump Piece Set");
                                //if the jumping piece can't continue end the turn
                                if (!board.getJumpingPiece().canJump())
                                {
                                    currentPlayerTurn = (currentPlayerTurn == Piece.PLAYER.BLACK) ? Piece.PLAYER.WHITE : Piece.PLAYER.BLACK; //switch the turn
                                    board.setJumpingPiece(null);
                                    board.setJumpAvailable(Piece.PLAYER.BLACK, false);board.setJumpAvailable(Piece.PLAYER.WHITE, false);;
                                }
                                else
                                {
                                    //if you are selecting the jumping piece and you are attempting a jump then jump
                                    if (board.getPiece(mouseClickedPiece.getCoords()) == board.getJumpingPiece() && (Math.Abs(mouseClickedPiece.getCoords().X - mouseBoardPosition.X) == 2))
                                    {
                                        board.movePiece(mouseClickedPiece.getCoords(), mouseBoardPosition); //Move the piece in the array to the spot where the mouse dropped it
                                        board.setJumpingPiece(board.getPiece(mouseBoardPosition));
                                        //remove the piece that got jumped
                                        board.removePiece((Math.Min((int)mouseClickedPiece.getCoords().X, (int)mouseBoardPosition.X) + 1)
                                        , (Math.Min((int)mouseClickedPiece.getCoords().Y, (int)mouseBoardPosition.Y) + 1));
                                    }
                                }
                            }
                            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            ///Case 2: Jump Available && Jump Piece NOT Set
                            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            //if there is a jump available and there is no piece currently jumping then make the jump and set which piece is jumping
                            else if (((board.getJumpAvailable(Piece.PLAYER.WHITE) && currentPlayerTurn == Piece.PLAYER.WHITE)
                                || (board.getJumpAvailable(Piece.PLAYER.BLACK) && currentPlayerTurn == Piece.PLAYER.BLACK)) 
                                && board.getJumpingPiece() == null)
                            {
                                Console.WriteLine(currentPlayerTurn + "Case 2: Jump Available && Jump Piece NOT Set");
                                //if you can jump with the selected piece and you are actually attempting to move far enough to make a jump
                                if ((board.getPiece(mouseClickedPiece.getCoords()).canJump()) && (Math.Abs(mouseClickedPiece.getCoords().X - mouseBoardPosition.X) == 2))
                                {
                                    board.movePiece(mouseClickedPiece.getCoords(), mouseBoardPosition); //Move the piece in the array to the spot where the mouse dropped it
                                    board.setJumpingPiece(board.getPiece(mouseBoardPosition));
                                    //remove the piece that got jumped
                                    board.removePiece((Math.Min((int)mouseClickedPiece.getCoords().X, (int)mouseBoardPosition.X) + 1)
                                    , (Math.Min((int)mouseClickedPiece.getCoords().Y, (int)mouseBoardPosition.Y) + 1));
                                    Console.WriteLine(currentPlayerTurn + " " + board.getJumpAvailable()[0] + " " + board.getJumpAvailable()[1]);
                                    setValidMovements(board); // set the valid movements again to see if there is another jump possible after doing a jump
                                    Console.WriteLine(currentPlayerTurn + " " + board.getJumpAvailable()[0] + " " + board.getJumpAvailable()[1]);
                                }
                            }
                            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            ///Case 3: Jump NOT Available && Jump Piece NOT Set
                            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            //if there is no jump available and no piece has jumped yet then move normally (not jumping) and change the turn
                            else if ((!(board.getJumpAvailable(Piece.PLAYER.WHITE) && currentPlayerTurn == Piece.PLAYER.WHITE)
                                && !(board.getJumpAvailable(Piece.PLAYER.BLACK) && currentPlayerTurn == Piece.PLAYER.BLACK))
                                && board.getJumpingPiece() == null)
                            {
                                Console.WriteLine(currentPlayerTurn + "Case 3: Jump NOT Available && Jump Piece NOT Set");
                                board.movePiece(mouseClickedPiece.getCoords(), mouseBoardPosition); //Move the piece in the array to the spot where the mouse dropped it
                                currentPlayerTurn = (currentPlayerTurn == Piece.PLAYER.BLACK) ? Piece.PLAYER.WHITE : Piece.PLAYER.BLACK; //switch the turn
                                board.setJumpAvailable(Piece.PLAYER.BLACK, false);board.setJumpAvailable(Piece.PLAYER.WHITE, false);;
                                board.setJumpingPiece(null);
                            }
                            
                            //The TEMPORARY FIX is no longer needed since I fixed it. 
                            //It is still here just in case I need to re add it

                            /*
                            ///TEMPORARY FIX
                            ///XxxXXxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            ///Hack fix for pieces not being removed after being jumped.
                            ///Idk why they aren't clearing but this will clear them if the logic missed them
                            ///BELIEF: Somehow jump moves are being handled by Case 3 incorrectly. 
                            ///It thinks there is no jump available when there is.
                            ///REASON: Jump available is stored as a single value even though
                            ///there are two jumps available at once. eg white might have a jump
                            ///but it records jump available as being for black
                            ///TODO: Make the right case accept the jumps to make sure they clear
                            ///XxxXXxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            //if the piece moved two x positions over that means it jumped.
                            if (Math.Abs(mouseClickedPiece.getCoords().X - mouseBoardPosition.X) == 2)
                            {
                                Console.WriteLine("uber haxxing");
                                // Tell the board which piece is jumping
                                board.setJumpingPiece(board.getPiece((int) mouseBoardPosition.X, (int)mouseBoardPosition.Y));
                                // remove the piece it jumped over
                                Math.Min((int)mouseClickedPiece.getCoords().X, (int)mouseBoardPosition.X);
                                board.removePiece((Math.Min((int)mouseClickedPiece.getCoords().X, (int)mouseBoardPosition.X) + 1)
                                    , (Math.Min((int)mouseClickedPiece.getCoords().Y, (int)mouseBoardPosition.Y) + 1));
                                
                            }*/


                            setValidMovements(board); // update the whole board's valid movements to ensure every piece knows that piece moved
                            //currentPlayerTurn = (currentPlayerTurn == Piece.PLAYER.BLACK)? Piece.PLAYER.WHITE : Piece.PLAYER.BLACK; //switch the turn
                        }
                    }
                }//end 4 direction for loop

                mouseClickedPiece = null;
                // Trigger the redrawing of pieces when a piece is dropped since it may have moved.
                pieceList.Clear(); // Clear the old locations of piece graphics.
                piecesCreated = false; // Tells the system that we will need to remake pieces.
                //Console.WriteLine("recreate piecelist");
                //Logic to corresspond the mouse X,Y coordinates with the board's index (0-7, 0-7)
                
                //Console.WriteLine(Math.Round( (mousePos.X - board_SquareSize) / board_SquareSize) + " " + Math.Round(Math.Abs(mousePos.Y / (board_SquareSize) - 8)));
            }//end mouse release
            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            ///Case 4: Jump NOT Available && Jump Piece Set
            ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //if you already jumped and you can't jump anymore, your turn is over
            if ((!(board.getJumpAvailable(Piece.PLAYER.WHITE) && currentPlayerTurn == Piece.PLAYER.WHITE)
                && !(board.getJumpAvailable(Piece.PLAYER.BLACK) && currentPlayerTurn == Piece.PLAYER.BLACK))
                && board.getJumpingPiece() != null)
            {
                Console.WriteLine(currentPlayerTurn + "Case 4: Jump NOT Available && Jump Piece Set");
                currentPlayerTurn = (currentPlayerTurn == Piece.PLAYER.BLACK) ? Piece.PLAYER.WHITE : Piece.PLAYER.BLACK; //switch the turn
                board.setJumpingPiece(null);
                board.setJumpAvailable(Piece.PLAYER.BLACK, false);board.setJumpAvailable(Piece.PLAYER.WHITE, false);;
            }

            // True if the mouse is pressed in the current frame.
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released)
            {
                // Look through all pieces
                foreach (View_Clickable thisPiece in pieceList)
                {
                    // If the current player is clicking one of his own pieces.
                    if (thisPiece.IsIntersected(mousePos) && (currentPlayerTurn == board.getPiece((int)thisPiece.getCoords().X, (int)thisPiece.getCoords().Y).getOwner()))
                    { 
                        mouseClickedPiece = thisPiece;
                        mouseOffset = thisPiece.getPosition() - mousePos;

                        // temporary console writing for testing
                        /*for (int i = 0; i < 4; i++)
                        {
                            Piece.validMovementsStruct vms = board.getPiece((int)thisPiece.getCoords().X, (int)thisPiece.getCoords().Y).getValidMovements()[i];
                            Console.WriteLine(vms.direction + " " + vms.col + " " + vms.row);
                        }*/
                        break; // Break to only pick up one piece at a time.
                    }
                }

                // MENU buttons events to switch between states
                if (currentState == STATE.MENU)
                {
                    if (clickable_CustomButton.IsIntersected(mousePos))
                    {
                        pieceList.Clear(); // Clear the old locations of piece graphics.
                        piecesCreated = false; // Tells the system that we will need to remake pieces.
                        currentState = STATE.SETUP;
                    }
                    if (clickable_PlayButton.IsIntersected(mousePos))
                    {
                        pieceList.Clear(); // Clear the old locations of piece graphics.
                        piecesCreated = false; // Tells the system that we will need to remake pieces.
                        currentState = STATE.PLAYING;
                    }
                    if (clickable_LoadButton.IsIntersected(mousePos))
                    {
                        pieceList.Clear(); // Clear the old locations of piece graphics.
                        piecesCreated = false; // Tells the system that we will need to remake pieces.
                        currentState = STATE.LOAD;
                    }
                }
                if (currentState == STATE.PLAYING)
                {
                    if (clickable_MenuButton.IsIntersected(mousePos))
                        currentState = STATE.MENU;
                    if (clickable_SaveButton.IsIntersected(mousePos))
                        fileIO.save(board, currentPlayerTurn); 
                }    
            }

            // If we haven't released the mouse, continue updating the position of the piece (for dragging).
            if (mouseClickedPiece != null) mouseClickedPiece.setPosition(mousePos + mouseOffset);

            base.Update(gameTime);
        }


        /// <summary>
        /// The game draws different things depending on $currentState.
        /// This method contains the view and graphics.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.IndianRed);

            spriteBatch.Begin(); // drawing goes after this line

            if (currentState == STATE.MENU) 
            {
                // draw menu
                clickable_PlayButton.Draw(spriteBatch);
                clickable_CustomButton.Draw(spriteBatch);
                clickable_LoadButton.Draw(spriteBatch);
            }

            else if (currentState == STATE.SETUP)
            {
                if(input == null)
                    takeInput();
            }

            else if (currentState == STATE.LOAD)
            {
                try {
                    string[] tempIO = fileIO.load(board);
                    board.setUpBoard(tempIO[1]);
                    if (tempIO[0] == "WHITE")
                    {
                        currentPlayerTurn = Piece.PLAYER.WHITE;
                    }
                    else if (tempIO[0] == "BLACK")
                    {
                        currentPlayerTurn = Piece.PLAYER.BLACK;
                    }
                    Console.WriteLine("Game Loaded!");
                    currentState = STATE.PLAYING;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Load error: " + e.Message);
                    currentState = STATE.MENU;
                } 
                 
            }

            else if (currentState == STATE.PLAYING)
            {
                // draw button for returning to menu
                clickable_MenuButton.Draw(spriteBatch);
                clickable_SaveButton.Draw(spriteBatch);

                // Loops through each location of the board only once per frame.
                // Inside the loop, the tile and piece is drawn at the same time.

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

                        // Draw tile
                        spriteBatch.Draw(tile, new Vector2((GraphicsDevice.Viewport.Width - board_SquareSize * 8) / 2 + board_SquareSize * col, (GraphicsDevice.Viewport.Height - board_SquareSize * 8) / 2 + board_SquareSize * row), // center the board relative to window size
                                null, Color.White, 0f, new Vector2(0, 0), board_squareScale, SpriteEffects.None, 0);

                        // Define each piece once by wrapping each piece in a class called View_Clickable 
                        // for the system to apply additional methods.
                        if (piecesCreated == false) {
                            if (board.getPiece(col, 7 - row) != null) 
                            {
                                Texture2D pieceTexture;
                                if (board.getPiece(col, 7 - row).getOwner() == Piece.PLAYER.BLACK) {
                                    switch (board.getPiece(col, 7 - row).getType()) // Chose the texture required for which piece.
                                    {
                                        case (Piece.TYPESTATE.NORMAL):
                                            pieceTexture = Piece_BlackNormal;
                                            break;
                                        case (Piece.TYPESTATE.KING):
                                            pieceTexture = Piece_BlackKing;
                                            break;
                                        default:
                                            throw new Exception("Error: Non existent piece type.");
                                    }
                                }
                                else if (board.getPiece(col, 7 - row).getOwner() == Piece.PLAYER.WHITE)
                                {
                                    switch (board.getPiece(col, 7 - row).getType())
                                    {
                                        case (Piece.TYPESTATE.NORMAL):
                                            pieceTexture = Piece_WhiteNormal;
                                            break;
                                        case (Piece.TYPESTATE.KING):
                                            pieceTexture = Piece_WhiteKing;
                                            break;
                                        default:
                                            throw new Exception("Error: Non existent piece type.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error: Piece owned by non existant player!");
                                }


                                // prepare a list of all pieces so we can draw them later
                                pieceList.Add(new View_Clickable(pieceTexture, new Vector2((GraphicsDevice.Viewport.Width - board_SquareSize * 8) / 2 + board_SquareSize * col + board_SquareSize / 2 - Piece_BlackNormal.Width / 2,
                                    (GraphicsDevice.Viewport.Height - board_SquareSize * 8) / 2 + board_SquareSize * row + board_SquareSize / 2 - Piece_BlackNormal.Height / 2), Color.White, 1f, col, 7 - row));
                                // need to save coordinates when making Piece views to know where they are

                                setValidMovements(board, col, 7 - row); // For effiency, the piece valid movements are updated within the same loop as drawing them.
                            }
                        }
                    }
                }
                piecesCreated = true; // Set true for the PLAYING state to not create brand new copies of pieces every frame.
                input = null; // Reset the input once done with drawing all pieces.

                // Every frame, draw pieces.
                foreach (View_Clickable sprite in pieceList) // we tell each piece to draw // actually, we can do this right after adding them to the list
                {
                    sprite.Draw(spriteBatch);
                }
            } // END OF PLAYING STATE


            spriteBatch.End(); // Drawing goes before this XNA line.
            base.Draw(gameTime);
        }
        /// <summary>
        /// This function will ask the user for a string to parse as the board setup
        /// There are no arguments
        /// The input string will be null before entering the function
        /// </summary>
        void takeInput()
        {
            input = "setup board"; // If the input is not null the function will not be called every frame.
            Console.WriteLine("Input a board in the format of A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B" + "\n" +
                                "No more that 12 of each colour, and only place on solid squares");
            input = Console.ReadLine();
            //input = "A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B"; // sample input
            try
            {
                board.setUpBoard(input); // this construction is able to throw exceptions 
                // the following code only runs if there were no exceptions above
                Console.WriteLine("Setting up board: " + input + "\n" + "Enjoy your game");
                currentState = STATE.PLAYING;
            }
            catch
            {
                Console.WriteLine("Please format the input correctly\n");
                takeInput();
            }
        }

        
        /// <summary>
        /// Sets the valid movements for every Piece
        /// Default constructor to set them for the entire board. The safe locations to move to are stored within the Pieces
        /// The valid locations are assigned in the order: Top Left -> Top Right -> Bottom Right -> Bottom Left
        /// the default constructor allows for calling the function with no paramaters to set up the entire board
        /// </summary>
        /// 

        public void setValidMovements(Board board)
        {
            board.setJumpAvailable(Piece.PLAYER.BLACK, false);board.setJumpAvailable(Piece.PLAYER.WHITE, false);;
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    setValidMovements(board, row, col);
                }
            }
        }

        public void setValidMovements(Board board, int x, int y)
        {
            // the valid movements are a combination of an x direction and a y direction. initialized to a flag of an unreachable location
            int newUpLeftX = -99, newUpLeftY = -99;
            int newUpRightX = -99, newUpRightY = -99;
            int newDownRightX = -99, newDownRightY = -99;
            int newDownLeftX = -99, newDownLeftY = -99;

            // Whether the current movement is a jump
            bool newUpLeftIsAJump = false, newUpRightIsAJump = false, newDownRightIsAJump = false, newDownLeftIsAJump = false;

            // check if game is over
            if (board.getNumPieces(Piece.PLAYER.BLACK) == 0)
                win(Piece.PLAYER.WHITE);
            else if (board.getNumPieces(Piece.PLAYER.WHITE) == 0)
                win(Piece.PLAYER.BLACK);

            if (board.getPiece(x, y) != null)
            {
                // logic to king (verb) pieces
                if (((y == 7 && board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE) || (y == 0 && board.getPiece(x, y).getOwner() == Piece.PLAYER.BLACK)) && board.getPiece(x, y).getType() == Piece.TYPESTATE.NORMAL)
                {
                    board.getPiece(x, y).setType(Piece.TYPESTATE.KING);
                }

                // valid movements logic

                // if you are a white piece, or black king. (this is upwards movement, only these types of pieces can move up)
                if ((board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE)
                    || (board.getPiece(x, y).getType() == Piece.TYPESTATE.KING && board.getPiece(x, y).getOwner() == Piece.PLAYER.BLACK))
                {
                    if (x - 1 >= 0) newUpLeftX = x - 1;
                    if (y + 1 <= 7) { newUpLeftY = y + 1; newUpRightY = y + 1; }
                    if (x + 1 <= 7) newUpRightX = x + 1;

                    //check to prevent capturing own pieces
                    //if the newUp/newLeft etc variables are positive, then they have valid values by this point
                    if (newUpLeftX >= 0 && newUpLeftY >= 0 && board.getPiece(newUpLeftX, newUpLeftY) != null)
                    {
                        if ((board.getPiece(newUpLeftX, newUpLeftY).getOwner() == Piece.PLAYER.WHITE && board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE)
                            || (board.getPiece(newUpLeftX, newUpLeftY).getOwner() == Piece.PLAYER.BLACK && board.getPiece(x, y).getOwner() == Piece.PLAYER.BLACK))
                        {
                            newUpLeftX = -99;
                        }
                        //there is a piece to jump
                        else if ((board.getPiece(newUpLeftX, newUpLeftY).getOwner() == Piece.PLAYER.BLACK && board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE)
                            || (board.getPiece(newUpLeftX, newUpLeftY).getOwner() == Piece.PLAYER.WHITE && board.getPiece(x, y).getOwner() == Piece.PLAYER.BLACK))
                        {
                            //Calculate jump to location if it is reachable
                            if (newUpLeftX - 1 >= 0 && newUpLeftY + 1 <= 7 && board.getPiece(newUpLeftX - 1, newUpLeftY + 1) == null)
                            {
                                newUpLeftX --;
                                newUpLeftY ++;
                                newUpLeftIsAJump = true;
                                //set the jump availibility. player 1 is white, player 2 is black
                                if(board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE) board.setJumpAvailable(Piece.PLAYER.WHITE, true);
                                else board.setJumpAvailable(Piece.PLAYER.BLACK, true);
                            }
                            //if the landing space is non empty the jump isnt valid mark it as such
                            else
                            {
                                newUpLeftX = -99;
                                newUpLeftY = -99;
                            }
                        }
                    }
                    if (newUpRightX >= 0 && newUpRightY >= 0 && board.getPiece(newUpRightX, newUpRightY) != null)
                    {
                        if (board.getPiece(x, y).getOwner() == board.getPiece(newUpRightX, newUpRightY).getOwner())
                        {
                            newUpRightX = -99;
                        }
                        //there is a piece to jump
                        else if (board.getPiece(x, y).getOwner() != board.getPiece(newUpRightX, newUpRightY).getOwner())
                        {
                            //Calculate jump to location
                            if (newUpRightX + 1 <= 7 && newUpRightY + 1 <= 7 && board.getPiece(newUpRightX + 1, newUpRightY + 1) == null)
                            {
                                newUpRightX ++;
                                newUpRightY ++;
                                newUpRightIsAJump = true;
                                //set the jump availibility. player 1 is white, player 2 is black
                                if (board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE) board.setJumpAvailable(Piece.PLAYER.WHITE, true);
                                else board.setJumpAvailable(Piece.PLAYER.BLACK, true);
                            }
                            //if the landing space is non empty the jump isnt valid mark it as such
                            else 
                            {
                                newUpRightX = -99;
                                newUpRightY = -99;
                            }
                        }
                    }
                }
                // if you are a black piece, or white king. (this is downwards movement, only these types of pieces can move down)
                // kings will have their movements set by the logic of the above if statement combined with this one. Normal Pieces just use one or the other
                if ((board.getPiece(x, y).getOwner() == Piece.PLAYER.BLACK)
                    || (board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE && board.getPiece(x, y).getType() == Piece.TYPESTATE.KING))
                {
                    if (x - 1 >= 0) newDownLeftX = x - 1;
                    if (x + 1 <= 7) newDownRightX = x + 1;
                    if (y - 1 >= 0) { newDownRightY = y - 1; newDownLeftY = y - 1; }

                    //check to prevent capturing own pieces
                    //if the newUp/newLeft etc variables are positive, then they have valid values by this point
                    if (newDownLeftX >= 0 && newDownLeftY >= 0 && board.getPiece(newDownLeftX, newDownLeftY) != null)
                    {
                        if (board.getPiece(x, y).getOwner() == board.getPiece(newDownLeftX, newDownLeftY).getOwner())
                        {
                            newDownLeftX = -99;
                        }
                        //there is a piece to jump
                        else if (board.getPiece(x, y).getOwner() != board.getPiece(newDownLeftX, newDownLeftY).getOwner())
                        {
                            //Calculate jump to location
                            if (newDownLeftX - 1 >= 0 && newDownLeftY - 1 >= 0 && board.getPiece(newDownLeftX - 1, newDownLeftY - 1) == null)
                            {
                                newDownLeftX--;
                                newDownLeftY--;
                                newDownLeftIsAJump = true;
                                //set the jump availibility. player 1 is white, player 2 is black
                                if (board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE) board.setJumpAvailable(Piece.PLAYER.WHITE, true);
                                else board.setJumpAvailable(Piece.PLAYER.BLACK, true);
                            }
                            //if the landing space is non empty the jump isnt valid mark it as such
                            else
                            {
                                newDownLeftX = -99;
                                newDownLeftY = -99;
                            }
                        }
                    }
                    if (newDownRightX >= 0 && newDownRightY >= 0 && board.getPiece(newDownRightX, newDownRightY) != null)
                    {
                        if (board.getPiece(x, y).getOwner() == board.getPiece(newDownRightX, newDownRightY).getOwner())
                        {
                            newDownRightX = -99;
                        }
                        //there is a piece to jump
                        else if (board.getPiece(x, y).getOwner() != board.getPiece(newDownRightX, newDownRightY).getOwner())
                        {
                            //Calculate jump to location
                            if (newDownRightX + 1 <= 7 && newDownRightY - 1 >= 0 && board.getPiece(newDownRightX + 1, newDownRightY - 1) == null)
                            {
                                newDownRightX++;
                                newDownRightY--;
                                newDownRightIsAJump = true;
                                //set the jump availibility. player 1 is white, player 2 is black
                                if (board.getPiece(x, y).getOwner() == Piece.PLAYER.WHITE) board.setJumpAvailable(Piece.PLAYER.WHITE, true);
                                else board.setJumpAvailable(Piece.PLAYER.BLACK, true);
                            }
                            //if the landing space is non empty the jump isnt valid mark it as such
                            else
                            {
                                newDownRightX = -99;
                                newDownRightY = -99;
                            }
                        }
                    }
                }

                // the move locations have been figured out by this point. update the valid movements for each piece
                board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_LEFT, newUpLeftX, newUpLeftY, newUpLeftIsAJump);
                board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_RIGHT, newUpRightX, newUpRightY, newUpRightIsAJump);
                board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_RIGHT, newDownRightX, newDownRightY, newDownRightIsAJump); //the negative numbers indicate there is no valid movement on the board in this direction
                board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_LEFT, newDownLeftX, newDownLeftY, newDownLeftIsAJump);
            }
        }// end setValidMovements function

        /// <summary>
        /// This function is called to say when the game has been won
        /// </summary>
        /// <param name="winner">This is the player that won</param>
        void win(Piece.PLAYER winner)
        {
            board.clear();
            Console.WriteLine(winner + " wins.");
            currentState = STATE.MENU;
        }
    }
    
}
