using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Linq;
using System;

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
        Vector2 position; // sokoban position

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

            // remove Sokoban from board, and return coordinates
            position = positionSokoban();

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

            // sair se jogo acabou
            if (isWin()) Exit();

            KeyboardState keys = Keyboard.GetState();
            Vector2 movement = Vector2.Zero;
            if (keys.IsKeyDown(Keys.Down))
                movement = Vector2.UnitY;
            else if (keys.IsKeyDown(Keys.Up))
                movement = -Vector2.UnitY;
            else if (keys.IsKeyDown(Keys.Left))
                movement = -Vector2.UnitX;
            else if (keys.IsKeyDown(Keys.Right))
                movement = Vector2.UnitX;

            if (isCrate(position + movement))
            {
                if (!isCrate(position + 2*movement) &&
                    !isWall(position + 2*movement))
                {
                    moveCrate(position + movement, position + 2 * movement);
                    position = position + movement;
                }
            }
            else if (!isWall(position + movement))
            {
                position = position + movement;
            }

            base.Update(gameTime);
        }

        void moveCrate(Vector2 origem, Vector2 destino)
        {
            // 1. remover da origem
            if (board[(int)origem.X, (int)origem.Y] == '*')
                board[(int)origem.X, (int)origem.Y] = '.';
            else
                board[(int)origem.X, (int)origem.Y] = ' ';

            // 2. colocar no destino
            if (board[(int)destino.X, (int)destino.Y] == '.')
                board[(int)destino.X, (int)destino.Y] = '*';
            else
                board[(int)destino.X, (int)destino.Y] = '$';
        }

        bool isCrate(Vector2 pos)
        {
            return (board[(int)pos.X, (int)pos.Y] == '$')
                || (board[(int)pos.X, (int)pos.Y] == '*');
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
                        case '*':
                        case '$':
                            spriteBatch.Draw(crate, new Vector2(x * size, y * size), Color.White);
                            break;
                        case '#':
                            spriteBatch.Draw(wall, new Vector2(x * size, y * size), Color.White);
                            break;
                        default:
                            break;
                    }
                }
            }
            spriteBatch.Draw(sokoban, position * size, Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }

        bool isWall(Vector2 coord)
        {
            return board[(int)coord.X, (int)coord.Y] == '#';
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

        // Is the board complete? (no dots!)
        bool isWin()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (board[x, y] == '.')
                        return false;
            return true;
        }

    }
}
