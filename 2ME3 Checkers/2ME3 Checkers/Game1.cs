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
        public enum PLAYER_TURN { PLAYER_1, PLAYER_2, AI }; // Each game will have two of these possible players. The AI is currently unused, and is for assignment 3
        private PLAYER_TURN currentPlayerTurn = PLAYER_TURN.PLAYER_1; 
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
                currentPlayerTurn = PLAYER_TURN.PLAYER_1;
            else if (keyState.IsKeyDown(Keys.W))
                currentPlayerTurn = PLAYER_TURN.PLAYER_2;

            //TEMP
            else if (keyState.IsKeyDown(Keys.K))
                setValidMovements(board);

            // Mouse Update Stuff
            mouseStatePrev = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();
            mousePos = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
            mouseBoardPosition = new Vector2((float) Math.Round( (mousePos.X - board_SquareSize) / board_SquareSize) 
                , (float) Math.Round(Math.Abs(mousePos.Y / (board_SquareSize) - 8))); // this is where the mouse is looking on the board ie (0-7), (0-7)
            
            // Drop piece if mouse button is up
            if (mouseStateCurrent.LeftButton == ButtonState.Released && mouseClickedPiece != null)
            {
                // there are 4 possible directions of movement
                for(int i = 0; i < 4; i++) {
                    //need a this conditional because board.getPiece() can return null if there is no Piece there
                    if(board.getPiece(mouseClickedPiece.getCoords()) != null)
                    {
                        //if the place where the mouse is releasing the piece is a valid move for the piece 
                        if ( (board.getPiece(mouseClickedPiece.getCoords()).getValidMovements()[i].col == mouseBoardPosition.X)
                            && (board.getPiece(mouseClickedPiece.getCoords()).getValidMovements()[i].row == mouseBoardPosition.Y) )
                        {
                            Console.WriteLine("move ok. because " + board.getPiece(mouseClickedPiece.getCoords()).getValidMovements()[i].col + "=" + mouseBoardPosition.X + " and ");
                            Console.WriteLine(board.getPiece(mouseClickedPiece.getCoords()).getValidMovements()[i].row + "=" + mouseBoardPosition.Y + "/n");

                            board.movePiece(mouseClickedPiece.getCoords(), mouseBoardPosition); //Move the piece in the array to the spot where the mouse dropped it
                            setValidMovements(board, (int)mouseBoardPosition.X, (int)mouseBoardPosition.Y); //Update that piece's valid movements
                            currentPlayerTurn = (currentPlayerTurn == PLAYER_TURN.PLAYER_1)? PLAYER_TURN.PLAYER_2 : PLAYER_TURN.PLAYER_1; //switch the turn
                        }
                    }
                }


                mouseClickedPiece = null;

                //Console.WriteLine("last sel piece was " + board.getPiece(
                pieceList.Clear(); // clear the board first
                piecesCreated = false;
                //Logic to corresspond the mouse X,Y coordinates with the board's index (0-7, 0-7)
                
                //Console.WriteLine(Math.Round( (mousePos.X - board_SquareSize) / board_SquareSize) + " " + Math.Round(Math.Abs(mousePos.Y / (board_SquareSize) - 8)));
            }

            // True if we pressed the mouse this frame.
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released)
            {
               
                // Look through all pieces
                foreach (View_Clickable thisPiece in pieceList)
                {
                    
                    Console.WriteLine(thisPiece.getCoords().ToString());
                    if (thisPiece.IsIntersected(mousePos)
                       && 
                        ((currentPlayerTurn == PLAYER_TURN.PLAYER_1 && board.getPiece((int)thisPiece.getCoords().X, (int)thisPiece.getCoords().Y).getOwner() == Piece.player.BLACK)
                        || (currentPlayerTurn == PLAYER_TURN.PLAYER_2 && board.getPiece((int)thisPiece.getCoords().X, (int)thisPiece.getCoords().Y).getOwner() == Piece.player.WHITE))
                        )// if we are clicking on the piece
                    {
                        mouseClickedPiece = thisPiece;
                        mouseOffset = thisPiece.getPosition() - mousePos;

                        //temporary console writign for testing
                        for (int i = 0; i < 4; i++)
                        {
                            Piece.validMovementsStruct vms = board.getPiece((int)thisPiece.getCoords().X, (int)thisPiece.getCoords().Y).getValidMovements()[i];
                            Console.WriteLine(vms.direction + " " + vms.col + " " + vms.row);
                        }
                        break; // break so we can only pick up one piece at a time
                    }
                }

                // MENU buttons events to switch between states
                if (currentState == STATE.MENU)
                {
                    if (clickable_CustomButton.IsIntersected(mousePos))
                        currentState = STATE.SETUP;
                    if (clickable_PlayButton.IsIntersected(mousePos))
                    {
                        currentState = STATE.PLAYING;
                    }
                    if (clickable_LoadButton.IsIntersected(mousePos))
                    {
                        currentState = STATE.LOAD;
                    }
                }
                if (currentState == STATE.PLAYING)
                {
                    if (clickable_MenuButton.IsIntersected(mousePos))
                        currentState = STATE.MENU;
                    if (clickable_SaveButton.IsIntersected(mousePos))
                        fileIO.save(board, currentPlayerTurn); // need a save STATE
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
                // draw setup
                // set up new board
                pieceList.Clear(); // clear the board first
                piecesCreated = false; // reset pieces
                
                if(input == null)
                    takeInput();
            }

            else if (currentState == STATE.LOAD)
            {
                pieceList.Clear(); // clear the board first
                piecesCreated = false; // reset pieces
                
                try {
                    string[] tempIO = fileIO.load(board);
                    board.setUpBoard(tempIO[1]);
                    if (tempIO[0] == "WHITE")
                    {
                        currentPlayerTurn = PLAYER_TURN.PLAYER_2;
                    }
                    else if (tempIO[1] == "BLACK")
                    {
                        currentPlayerTurn = PLAYER_TURN.PLAYER_1;
                    }
                    Console.WriteLine("Game Loaded!");
                    currentState = STATE.PLAYING;
                } 
                catch 
                {
                    Console.WriteLine("No Save Game!");
                    currentState = STATE.MENU;
                } 
                 
            }

            else if (currentState == STATE.PLAYING)
            {
                // draw button for returning to menu
                clickable_MenuButton.Draw(spriteBatch);
                clickable_SaveButton.Draw(spriteBatch);

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
                        spriteBatch.Draw(tile, new Vector2((GraphicsDevice.Viewport.Width - board_SquareSize * 8) / 2 + board_SquareSize * col, (GraphicsDevice.Viewport.Height - board_SquareSize * 8) / 2 + board_SquareSize * row), // center the board relative to window size
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
                                pieceList.Add(new View_Clickable(pieceTexture, new Vector2((GraphicsDevice.Viewport.Width - board_SquareSize * 8) / 2 + board_SquareSize * col + board_SquareSize / 2 - Piece_BlackNormal.Width / 2,
                                    (GraphicsDevice.Viewport.Height - board_SquareSize * 8) / 2 + board_SquareSize * row + board_SquareSize / 2 - Piece_BlackNormal.Height / 2), Color.White, 1f, col, 7-row)); // 32 offsets again, maybe put these into a variable
                                //need to save coordinates when making Piece views to know where they are
                            }
                            setValidMovements(board);
                            Console.WriteLine("WE SET VALID MOVEMENTS");
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
                board.setUpBoard(input); // this construction is able to throw exceptions 
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
        void takeInput(String input) {
            if (input != "no save file") {
                board.setUpBoard(input);
                currentState = STATE.PLAYING;
            }
        }



        //Sets the valid movements for a specific Piece
        
        /// <summary>
        /// Sets the valid movements for every Piece
        /// Default constructor to set them for the entire board. The safe locations to move to are stored within the Pieces
        /// The valid locations are assigned in the order: Top Left -> Top Right -> Bottom Right -> Bottom Left
        /// the default constructor allows for calling the function with no paramaters to set up the entire board
        /// </summary>
        /// 

        void setValidMovements(Board board)
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    setValidMovements(board, row, col);
                }
            }
        }

        void setValidMovements(Board board, int x, int y)
        {
            if (board.getPiece(x, y) != null)
            {
                try
                {
                    if (board.getPiece(x, y).getType() == Piece.typeState.NORMAL)
                    { 
                        if (board.getPiece(x, y).getOwner() == Piece.player.WHITE)
                        {

                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_LEFT, x - 1, y + 1);
                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_RIGHT, x + 1, y + 1);
                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_RIGHT, -99, -99); //the negative numbers indicate there is no valid movement on the board in this direction
                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_LEFT, -99, -99);

                            if (y == 7)
                            {
                                board.getPiece(x, y).setType(Piece.typeState.KING);
                                setValidMovements(board, x, y);
                        }
                        }
                        else if (board.getPiece(x, y).getOwner() == Piece.player.BLACK)
                        {

                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_LEFT, -99, -99);
                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_RIGHT, -99, -99);
                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_RIGHT, x + 1, y - 1); //the negative numbers indicate there is no valid movement on the board in this direction
                            board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_LEFT, x - 1, y - 1);

                            if (y == 0)
                            {
                                board.getPiece(x, y).setType(Piece.typeState.KING);
                                setValidMovements(board, x, y);
                        }
                    }
                    }
                    else if (board.getPiece(x, y).getType() == Piece.typeState.KING)
                    {
                        board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_LEFT, x - 1, y + 1);
                        board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.UP_RIGHT, x + 1, y + 1);
                        board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_RIGHT, x + 1, y - 1); //the negative numbers indicate there is no valid movement on the board in this direction
                        board.getPiece(x, y).setValidMovements(Piece.validMoveDirection.DOWN_LEFT, x - 1, y - 1);
                    }
                }
                catch { throw new Exception("Error: trying to set a valid movement to a piece not in the board piece array"); }
            }
           
        }// end setValidMovements function
        
    }
    
}
