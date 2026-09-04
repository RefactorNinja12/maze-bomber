using BomberRoyal.Core;
using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.GameState;
using BomberRoyal.Core.Input;
using BomberRoyal.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BomberRoyal
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

      
        private RenderTarget2D _mazeTarget;

       
        private int _mazePixelWidth = 1280;
        private int _mazePixelHeight = 720;

        public static SpriteFont GlobalFont { get; private set; }
        public static TextureAtlas Atlas { get; private set; }
        public static ContentManager GlobalContent { get; private set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
              
                PreferMultiSampling = false
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            
            Window.AllowUserResizing = true;

            // När fönstrets klientyta ändras (storlek eller DPI-relaterat) så
            // behöver vi bara rita om med ny skala – ingen backbuffer-resize behövs
            Window.ClientSizeChanged += (_, __) =>
            {
                
            };
        }

        protected override void Initialize()
        {
           
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics.PreferredBackBufferWidth = (int)(display.Width * 0.70f);
            _graphics.PreferredBackBufferHeight = (int)(display.Height * 0.70f);
            _graphics.ApplyChanges();

            // Event-subscriptions
            var bus = GameManager.Instance.EventBus;
            bus.Subscribe<MazeGeneratedEvent>(OnMazeGenerated);
            bus.Subscribe<StateChangedEvent>(OnStateChanged);
            bus.Subscribe<ExitGameEvent>(OnExitGame);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalContent = Content;

            GlobalFont = Content.Load<SpriteFont>("DefaultFont");
            var atlasTexture = Content.Load<Texture2D>("Tileset");
            Atlas = new TextureAtlas(atlasTexture);
            Atlas.AddRegion(name: "border_tiles", x: 16, y: 16, width: 80, height: 80);

            // Skapa temporär RenderTarget tills Maze finns
            _mazeTarget = new RenderTarget2D(
                GraphicsDevice,
                _mazePixelWidth,
                _mazePixelHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );

            // Starta i meny
            SceneManager.Instance.SetScene(new MainMenuScene(GraphicsDevice, GameManager.Instance.EventBus));
            GameManager.Instance.Initialize();
            GameManager.Instance.ChangeState(new MainMenuState(GameManager.Instance));
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameManager.Instance.Update(dt);
            SceneManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_mazeTarget);
            GraphicsDevice.Clear(Color.Black);
            SceneManager.Instance.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);

            var client = Window.ClientBounds;
            int clientW = Math.Max(1, client.Width);
            int clientH = Math.Max(1, client.Height);

            float scaleX = (float)clientW / _mazeTarget.Width;
            float scaleY = (float)clientH / _mazeTarget.Height;
            float finalScale = Math.Min(scaleX, scaleY);

            int drawW = (int)(_mazeTarget.Width * finalScale);
            int drawH = (int)(_mazeTarget.Height * finalScale);

            int offsetX = (clientW - drawW) / 2;

      
            int offsetY = (clientH - drawH) / 2;

            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(
                _mazeTarget,
                destinationRectangle: new Rectangle(offsetX, offsetY, drawW, drawH),
                color: Color.White
            );

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private void OnMazeGenerated(MazeGeneratedEvent e)
        {
            var maze = e.Maze;
            if (maze == null) return;

            const int tileSize = 42;
            _mazePixelWidth = maze.Width * tileSize;
            _mazePixelHeight = maze.Height * tileSize;

            // Skapa om RenderTarget i exakt Maze-upplösning
            _mazeTarget?.Dispose();
            _mazeTarget = new RenderTarget2D(
                GraphicsDevice,
                _mazePixelWidth,
                _mazePixelHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );

          
        }

        private void OnStateChanged(StateChangedEvent e)
        {
            if (e.NewState is PlayingState)
                SceneManager.Instance.SetScene(new GameScene(GraphicsDevice));
            else if (e.NewState is GameOverState)
                SceneManager.Instance.SetScene(new GameOverScene(GraphicsDevice));
            else if (e.NewState is MainMenuState)
                SceneManager.Instance.SetScene(new MainMenuScene(GraphicsDevice, GameManager.Instance.EventBus));
        }

        private void OnExitGame(ExitGameEvent _)
        {
            Exit();
        }
    }
}