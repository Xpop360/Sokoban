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

        bool win = false;

        char[,] board;
        int width, height;
        int size = 64; // tamanho (largura e altura) das imagens usadas
        Texture2D wall, crate, point, sand, pixel;

        float scale = 0.75f; // Escala 
        SpriteFont arialBlack20;

        int nrMovements = 0;

        int nrLevels = 2;
        int curLevel = 1;

        float movementTimer = 0f;

        Sokoban sokoban;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.Black });

            loadLevel();
            base.Initialize();
        }

        void loadLevel()
        {
            board = readSokoban(@"Content\level" + curLevel + ".sok");
            width = board.GetLength(0);
            height = board.GetLength(1);

            // remove Sokoban from board, and return coordinates
            sokoban = new Sokoban(Content, positionSokoban());

            nrMovements = 0;
            win = false;

            graphics.PreferredBackBufferHeight = 
                (int)(scale*(30 + height * size));
            graphics.PreferredBackBufferWidth =
                (int)(scale * width * size);
            graphics.ApplyChanges();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            wall = Content.Load<Texture2D>("wall");
            crate = Content.Load<Texture2D>("crate");
            point = Content.Load<Texture2D>("point");
            sand = Content.Load<Texture2D>("sand");

            arialBlack20 = Content.Load<SpriteFont>("ArialBlack_20");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            movementTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isWin())
            {
                if (curLevel < nrLevels)
                {
                    curLevel++;
                    loadLevel();
                }
                else
                    win = true;
            }

            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.R))
            {
                if (win) curLevel = 1;
                loadLevel();
            }

            // Se passou mais que 1/10 segundo desde ultima tecla
            if (!win && movementTimer > 1f / 10f)
            {
                Vector2 movement = Vector2.Zero;
                if (keys.IsKeyDown(Keys.Down))
                    movement = Vector2.UnitY;
                else if (keys.IsKeyDown(Keys.Up))
                    movement = -Vector2.UnitY;
                else if (keys.IsKeyDown(Keys.Left))
                    movement = -Vector2.UnitX;
                else if (keys.IsKeyDown(Keys.Right))
                    movement = Vector2.UnitX;

                // Se utilizador carregou numa tecla
                if (movement != Vector2.Zero)
                {
                    // reset timer
                    movementTimer = 0f;

                    Vector2 position = sokoban.Position();
                    if (isCrate(position + movement))
                    {
                        if (!isCrate(position + 2 * movement) &&
                            !isWall(position + 2 * movement))
                        {
                            moveCrate(position + movement, position + 2 * movement);
                            
                            sokoban.Move(movement);
                            nrMovements++;
                        }
                    }
                    else if (!isWall(position + movement))
                    {
                        nrMovements++;
                        sokoban.Move(movement);
                    }
                }
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
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale));

            spriteBatch.DrawString(arialBlack20,
                 "Moves: " + nrMovements, new Vector2(10, height*size-5),
                 Color.Yellow);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    spriteBatch.Draw(sand, new Vector2(x * size, y * size), Color.White);
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
            sokoban.Draw(spriteBatch);
            spriteBatch.End();

            if (win)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(pixel,
                    new Rectangle(0, 0, GraphicsDevice.Viewport.Width,
                                        GraphicsDevice.Viewport.Height),
                    new Color(Color.Black, 0.5f));

                Vector2 strSize = arialBlack20.MeasureString("YOU WIN!");
                spriteBatch.DrawString(arialBlack20,
                    "YOU WIN!",
                    (new Vector2(GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height) - strSize) * 0.5f,
                    Color.LightGreen);

                spriteBatch.End();
            }

            

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
