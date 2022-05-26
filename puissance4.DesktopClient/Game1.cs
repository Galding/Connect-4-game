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
        public const int VX = 6;
        public const int VY = 7;

        private int currentPlayer = 1;
        private int currentColumn = VY / 2;
        private bool is_draw = false;
        private SpriteFont font;
        private Board board;

        private State state = State.Menu;
        private Gamemode gamemode = Gamemode.Player;
        private Color textSwitchingColor = Color.Red;
        private int textSwitchingTimer = 0;

        private AI ai;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            board = new Board();
            ai = new AI();
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
            currentKeyState = Keyboard.GetState();
            switch (state)
            {
                case State.Menu:
                    UpdateMenu();
                    break;
                case State.Playing:
                    UpdateGame();
                    break;
                case State.End:
                    UpdateEndScreen();
                    break;
                default: break;
            }
            previousKeyState = currentKeyState;
            base.Update(gameTime);
        }

        protected void UpdateMenu()
        {
            if (IsKeyPressed(Keys.Down) || IsKeyPressed(Keys.Up))
                gamemode = gamemode == Gamemode.Player ? Gamemode.AI : Gamemode.Player;

            if (IsKeyPressed(Keys.Enter))
                state = State.Playing;

            if (IsKeyPressed(Keys.Escape))
                Exit();
        }

        protected void UpdateGame()
        {
            if (gamemode == Gamemode.AI && currentPlayer == 2)
            {
                currentColumn = ai.Play();
                Play();
            }

            if (IsKeyPressed(Keys.Escape))
            {
                resetGame();
                state = State.Menu;
            }

            if (IsKeyPressed(Keys.Right) && currentColumn < VX)
                currentColumn++;

            if (IsKeyPressed(Keys.Left) && currentColumn > 0)
                currentColumn--;

            if (IsKeyPressed(Keys.Down))
            {
                Play();
            }
        }

        private void Play()
        {
            bool success = board.dropCoin(currentColumn, currentPlayer);
            if (success)
            {
                int winner = board.getWinner();
                if (winner == 0) switchPlayer();
                else
                {
                    is_draw = winner == -1;
                    state = State.End;
                }
            }
        }

        protected void UpdateEndScreen()
        {
            if (IsKeyPressed(Keys.R))
            {
                resetGame();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);
            switch (state)
            {
                case State.Menu:
                    DrawMenu();
                    break;
                case State.Playing:
                    DrawGame();
                    break;
                case State.End:
                    DrawGame();
                    DrawEndScreen();
                    break;
                default: break;
            }
            base.Draw(gameTime);
        }

        private void DrawMenu()
        {
            String title = "Connect 4 Game";
            _spriteBatch.Begin();
            Vector2 titlePos = new Vector2(Window.ClientBounds.Width / 2, 100);
            Vector2 textMiddlePoint = font.MeasureString(title) / 2;
            _spriteBatch.DrawString(font, title, titlePos, textSwitchingColor, 0, textMiddlePoint, 1.5f, SpriteEffects.None, 0.5f);
            String[] options = new String[] { "Player vs Player", "Player vs AI" };
            for (int i = 0; i < options.Length; i++)
            {
                Vector2 optionPos = new Vector2(Window.ClientBounds.Width / 2, 350 + i * 100);
                textMiddlePoint = font.MeasureString(options[i]) / 2;
                Color color = i == (int)gamemode ? textSwitchingColor : Color.White;
                _spriteBatch.DrawString(font, options[i], optionPos, color, 0, textMiddlePoint, 1f, SpriteEffects.None, 0.5f);
            }
            _spriteBatch.End();
            if (textSwitchingTimer > 10)
            {
                textSwitchingColor = textSwitchingColor == Color.Red ? Color.Yellow : Color.Red;
                textSwitchingTimer = 0;
            }
            textSwitchingTimer++;
        }

        private void DrawGame()
        {
            _spriteBatch.Begin();

            int offsetX = 100;
            int offsetY = 162;
            Vector2 arrow_pos = new Vector2(offsetY + currentColumn * 100, offsetX - 100);
            _spriteBatch.Draw(arrow.Texture, arrow_pos, currentPlayer == 1 ? Color.Yellow : Color.Red);
            for (int x = 0; x < VX; x++)
            {
                for (int y = 0; y < VY; y++)
                {
                    int xpos, ypos;
                    xpos = offsetX + x * 100;
                    ypos = offsetY + y * 100;
                    Vector2 pos = new Vector2(ypos, xpos);
                    switch (board.getValueAt(x, y))
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
        }

        private void DrawEndScreen()
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

        public void switchPlayer()
        {
            currentPlayer = currentPlayer == 1 ? 2 : 1;
        }
        public void resetGame()
        {
            state = State.Playing;
            for (int i = 0; i < Game1.VX; i++)
            {
                for (int j = 0; j < Game1.VY; j++)
                {
                    board.setValueAt(i, j, 0);
                }
            }
            switchPlayer();
            is_draw = false;
            currentColumn = Game1.VY / 2;
        }
    }
}
