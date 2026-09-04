using BomberRoyal.Core;
using BomberRoyal.Core.Event;
using BomberRoyal.Core.GameState;
using BomberRoyal.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace BomberRoyal.Scene
{
    public class MainMenuScene : IScene
    {
        private readonly IEventBus _eventBus;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font; 
        private readonly GraphicsDevice _graphics;

     
        public MainMenuScene(GraphicsDevice graphics, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _graphics = graphics;
        }
        public void Draw(GameTime gametime)
        {
            
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, "BomberRoyal", new Vector2(200, 200), Color.White);
            _spriteBatch.DrawString(_font, "Press Enter to start", new Vector2(200, 250), Color.Gray);
            _spriteBatch.DrawString(_font, "Press ESC to Quit", new Vector2(200, 280), Color.Gray);
            _spriteBatch.End();
        }

        public void LoadContent()
        {
            Console.WriteLine("MainMenuScene.LoadContent körs...");
            _spriteBatch = new SpriteBatch(_graphics);
            _font = Game1.GlobalFont;
        }

        public void Update(GameTime gametime)
        {
            var input = InputManager.Instance;
            if (input.isKeyPressed(Keys.Enter))
            {
                _eventBus.Publish(new StartGameEvent());
            }
            if(input.isKeyPressed(Keys.Escape))
            {
                ExitGame();
            }
            
        }
        private void ExitGame()
        {
            _eventBus.Publish(new ExitGameEvent());
        }
    }
}
