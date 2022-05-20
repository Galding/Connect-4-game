using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace puissance4.DesktopClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Puissance4Object white;
        private Puissance4Object red;
        private Puissance4Object yellow;
        private Puissance4Object arrow;
        private Texture2D endBackGroudTexture;
        const int VX = 6;
        const int VY = 7;
        private byte[,] map;
        private int currentPlayer = 1;
        private int currentColumn = VY / 2;
        private bool is_draw = false;
        private bool isPlaying = true;
        private SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            map = new byte[VX, VY]{ // 0 = empty (white), 1 = yellow, 2 = red
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0}
             };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            // on charge un objet mur
            white = new Puissance4Object(Content.Load<Texture2D>("white"), new Vector2(0f,
           0f), new Vector2(100f, 100f));
            yellow = new Puissance4Object(Content.Load<Texture2D>("yellow"), new
           Vector2(0f, 0f), new Vector2(100f, 100f));
            red = new Puissance4Object(Content.Load<Texture2D>("red"), new
           Vector2(0f, 0f), new Vector2(100f, 100f));
            arrow = new Puissance4Object(Content.Load<Texture2D>("arrow"), new Vector2(0f,
           0f), new Vector2(100f, 100f));
            font = Content.Load<SpriteFont>("ImpactFont");
            endBackGroudTexture = new Texture2D(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            Color[] color = new Color[_graphics.PreferredBackBufferWidth * _graphics.PreferredBackBufferHeight];
            for (int i = 0; i < color.Length; i++) color[i] = Color.Black;
            endBackGroudTexture.SetData(color);


        }

        private KeyboardState currentKeyState;
        private KeyboardState previousKeyState = new KeyboardState();
        protected bool IsKeyPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            this.currentKeyState = Keyboard.GetState();
            if (isPlaying)
            {
                if (IsKeyPressed(Keys.Right) && this.currentColumn < VX)
                {
                    this.currentColumn++;
                }
                if (IsKeyPressed(Keys.Left) && this.currentColumn > 0)
                {
                    this.currentColumn--;
                }
                if (IsKeyPressed(Keys.Down))
                {
                    bool success = this.dropCoin(this.currentColumn, this.currentPlayer);
                    if (success)
                    {
                        int winner = this.getWinner();
                        if (winner == 0) this.switchPlayer();
                        else
                        {
                            is_draw = winner == -1;
                            isPlaying = false;
                        }
                    }
                }
                this.previousKeyState = this.currentKeyState;
            }
            else
            {
                if (IsKeyPressed(Keys.R))
                {
                    resetGame();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            int offsetX = 100;
            int offsetY = 162;
            Vector2 arrow_pos = new Vector2(offsetY + this.currentColumn * 100, offsetX - 100);
            _spriteBatch.Draw(arrow.Texture, arrow_pos, this.currentPlayer == 1 ? Color.Yellow : Color.Red);
            for (int x = 0; x < VX; x++)
            {
                for (int y = 0; y < VY; y++)
                {
                    int xpos, ypos;
                    xpos = offsetX + x * 100;
                    ypos = offsetY + y * 100;
                    Vector2 pos = new Vector2(ypos, xpos);
                    switch (map[x, y])
                    {
                        case 0: // white
                            _spriteBatch.Draw(white.Texture, pos, Color.White);
                            break;
                        case 1: // yellow
                            _spriteBatch.Draw(yellow.Texture, pos, Color.White);
                            break;
                        case 2: // red
                            _spriteBatch.Draw(red.Texture, pos, Color.White);
                            break;
                        default:
                            break;
                    }
                }

            }

            _spriteBatch.End();
            if (!isPlaying)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(endBackGroudTexture, new Vector2(0, 0), Color.White * 0.5f);
                var winText = is_draw ? "Draw game!" : "You WIN!";
                Vector2 textMiddlePoint = font.MeasureString(winText) / 2;
                Vector2 winTextPosition = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
                Color textColor = Color.White;
                if (!is_draw) textColor = currentPlayer == 1 ? Color.Yellow : Color.Red;
                _spriteBatch.DrawString(font, winText, winTextPosition, textColor, 0, textMiddlePoint, 1.0f, SpriteEffects.None, 0.5f);
                Vector2 restartPos = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height - 75);
                _spriteBatch.DrawString(font, "Press R to restart", restartPos, Color.White);
                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        protected void switchPlayer()
        {
            this.currentPlayer = this.currentPlayer == 1 ? 2 : 1;
        }

        protected bool dropCoin(int column, int player)
        {
            if (this.map[0, column] != 0) // if column already full
                return false;

            int x = VX - 1;
            while (this.map[x, column] != 0) // found first empty from the bottom
                x--;

            this.map[x, column] = Convert.ToByte(player);
            return true;
        }

        protected int getWinner()
        {
            var winner = 0;
            for (int i = 0; i < VX; i++)
            {
                winner = getPlayerThatHasFourInArray(getColumn(i));
                if (winner != 0) return winner;
            }
            for (int i = 0; i < VY; i++)
            {
                winner = getPlayerThatHasFourInArray(getRow(i));
                if (winner != 0) return winner;
            }

            byte[][] diagonals = getDiagonals();
            foreach (var diagonal in diagonals)
            {
                winner = getPlayerThatHasFourInArray(diagonal);
                if (winner != 0) return winner;
            }

            if (isMapFull()) return -1;

            return 0;
        }

        private bool isMapFull()
        {
            for (int y = 0; y < VY; y++)
            {
                for (int x = 0; x < VX; x++)
                {
                    if (map[x, y] == 0) return false;
                }
            }
            return true;
        }

        protected int getPlayerThatHasFourInArray(byte[] array)
        {
            for (int p = 1; p <= 2; p++)
            {
                byte connected = 0;
                var previous = 0;
                foreach (byte current in array)
                {
                    if (current != p)
                    {
                        connected = 0;
                        continue;
                    }
                    if (current != previous)
                    {
                        connected = 1;
                        previous = current;
                        continue;
                    }
                    connected++;
                    if (connected == 4)
                    {
                        return p;
                    }
                }
            }
            return 0;
        }

        protected byte[] getColumn(int row)
        {
            var result = new byte[VY - 1];
            for (int i = 0; i < VY; i++)
            {
                result = result.Append(map[row, i]).ToArray();
            }
            return result;
        }

        protected byte[] getRow(int column)
        {
            var result = new byte[VX - 1];
            for (int i = 0; i < VX; i++)
            {
                result = result.Append(map[i, column]).ToArray();
            }
            return result;
        }

        protected byte[][] getDiagonals()
        {
            byte[][] diagonals = new byte[][] { };
            int x;
            for (int way = 0; way < 2; way++)
            {
                for (int offset = -VX + 1; offset < VX; offset++)
                {
                    byte[] diagonal = new byte[] { };
                    for (int y = 0; y < VY; y++)
                    {
                        x = way == 1 ? y + offset : VX - 1 - y + offset;
                        if (x < 0 || x > VX - 1 || y < 0 || y > VY) continue;
                        Array.Resize(ref diagonal, diagonal.Length + 1);
                        diagonal[diagonal.Length - 1] = map[x, y];
                    }
                    if (diagonal.Length >= 4)
                    {
                        Array.Resize(ref diagonals, diagonals.Length + 1);
                        diagonals[diagonals.Length - 1] = diagonal;
                    }
                }
            }
            return diagonals;
        }

        protected void resetGame()
        {
            isPlaying = true;
            for (int i = 0; i < VX; i++)
            {
                for (int j = 0; j < VY; j++)
                {
                    map[i, j] = 0;
                }
            }
            switchPlayer();
            is_draw = false;
            currentColumn = VY / 2;
        }
    }
}
