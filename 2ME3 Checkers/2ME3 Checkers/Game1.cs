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
    /// TODO: 
    ///   * Add mouse control
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
        Texture2D Piece_Normal;
        Texture2D Menu_ButtonPlay;

        private int board_SquareSize = 64; // pixel width to upscale to // keep it a multiple of the actual image
        private float board_squareScale;
 

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
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
            Board_SquareBlack = this.Content.Load<Texture2D>("textures/Board_SquareBlack"); // make sure these two squares are the same size
            Board_SquareWhite = this.Content.Load<Texture2D>("textures/Board_SquareWhite"); 
            board_squareScale = board_SquareSize / Board_SquareBlack.Height; // calculate the percent scaling we need to get the right square size
            Piece_Normal = this.Content.Load<Texture2D>("textures/Piece_Normal");
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
                spriteBatch.Draw(Piece_Normal, new Vector2(0, 0), Color.White);
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
                            tile = Board_SquareBlack;
                        }
                        else
                        {
                            tile = Board_SquareWhite;
                        }

                        // draw tile
                        spriteBatch.Draw(tile, new Vector2(32 + board_SquareSize * row, 32 + board_SquareSize * col), // the 32 is the gap to the left and top of the board
                                null, Color.White, 0f, new Vector2(0, 0), board_squareScale, SpriteEffects.None, 0);
                        // draw pieces on tile
                               
                    }
                }
            }

            spriteBatch.End(); // drawing goes before this line
            base.Draw(gameTime);
        }
    }
}
