using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberRoyal.Scene
{
    public class GameOverScene : IScene
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private readonly GraphicsDevice _graphics;
        public GameOverScene(){}
        public GameOverScene(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }
        public void LoadContent()
        {
            _spriteBatch = new SpriteBatch(_graphics);
            _font = Game1.GlobalFont;
        }
        public void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, "GAME OVER", new Vector2(200, 200), Color.Red);
            _spriteBatch.DrawString(_font, "Press R to Retry", new Vector2(200, 260), Color.LightGray);
            _spriteBatch.DrawString(_font, "Press Esc to Menu", new Vector2(200, 290), Color.LightGray);
            _spriteBatch.End();
        }
        public void Update(GameTime gameTime)
        {
           
        }
    }
}