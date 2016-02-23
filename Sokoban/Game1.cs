using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Linq;

namespace Sokoban
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        char[,] board;
        int width, height;
        int size = 64; // tamanho (largura e altura) das imagens usadas
        Texture2D wall, crate, sokoban, point;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            board = readSokoban(@"Content\level1.sok");
            width = board.GetLength(0);
            height = board.GetLength(1);

            graphics.PreferredBackBufferHeight = height * size;
            graphics.PreferredBackBufferWidth = width * size;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            wall = Content.Load<Texture2D>("wall");
            crate = Content.Load<Texture2D>("crate");
            sokoban = Content.Load<Texture2D>("sokoban");
            point = Content.Load<Texture2D>("point");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (board[x,y])
                    {
                        case '.':
                            spriteBatch.Draw(point, new Vector2(x * size, y * size), Color.White);
                            break;
                        case '$':
                            spriteBatch.Draw(crate, new Vector2(x * size, y * size), Color.White);
                            break;
                        case '@':
                            spriteBatch.Draw(sokoban, new Vector2(x * size, y * size), Color.White);
                            break;
                        case '#':
                            spriteBatch.Draw(wall, new Vector2(x * size, y * size), Color.White);
                            break;
                        default:
                            break;
                    }
                }
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }


        /*
        *  # - Parede
        *  . - destino de caixa
        *  $ - caixa
        *  @ - Sokoban
        *  * - caixa no destino
        *  + - sokoban num destino
        *  espaco, caminho...
        */

        static char[,] readSokoban(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            int width = lines.Select(x => x.Length).Max();
            int height = lines.Length;

            char[,] board = new char[width, height];

            for (int line = 0; line < height; line++)
            {
                for (int c = 0; c < width; c++)
                {
                    board[c, line] = c < lines[line].Length ? lines[line][c] : ' ';
                }
            }
            return board;
        }

        Vector2 positionSokoban()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (board[x,y] == '@')
                    {
                        board[x, y] = ' ';
                        return new Vector2(x, y);
                    }
                    else if (board[x,y] == '+')
                    {
                        board[x, y] = '.';
                        return new Vector2(x, y);
                    }
                }
            }
            return Vector2.Zero; // em principio, nunca executado
        }
    }
}
