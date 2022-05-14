using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace puissance4.DesktopClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Puissance4Object cadre;
        private Puissance4Object red;
        private Puissance4Object yellow;
        const int VX = 6;
        const int VY = 7;
        private byte[,] map;
        private string direction;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            map = new byte[VX, VY]{
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

            // TODO: use this.Content to load your game content here
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 660;
            _graphics.ApplyChanges();
            // on charge un objet mur
            cadre = new Puissance4Object(Content.Load<Texture2D>("white"), new Vector2(0f,
           0f), new Vector2(100f, 100f));
            yellow = new Puissance4Object(Content.Load<Texture2D>("yellow"), new
           Vector2(0f, 0f), new Vector2(100f, 100f));
            red = new Puissance4Object(Content.Load<Texture2D>("red"), new
           Vector2(0f, 0f), new Vector2(100f, 100f));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Right))
            {
                direction = "RIGHT";
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                direction = "LEFT";
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            int offsetX = 40;
            int offsetY = 140;
            for (int x = 0; x < VX; x++)
            {
                for (int y = 0; y < VY; y++)
                {
                    if (map[x, y] == 0)
                    {
                        int xpos, ypos;
                        xpos = offsetX + x * 100;
                        ypos = offsetY + y * 100;
                        Vector2 pos = new Vector2(ypos, xpos);
                        if (x == 0 && y == 0)
                        {
                            _spriteBatch.Draw(red.Texture, pos, Color.White);
                        }
                        else
                            _spriteBatch.Draw(cadre.Texture, pos, Color.White);
                    }
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
