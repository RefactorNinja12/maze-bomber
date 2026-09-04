using BomberRoyal.Core.Features.PlayerFeature;
using BomberRoyal.Core.Features.CurrencyFeature;
using BomberRoyal.Core.Features.BombFeature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberRoyal.Core.Features.UIFeature
{
    public class GameUI
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly Texture2D _pixel;
        private readonly Player _player;
        private readonly CurrencySystem _currency;
        private readonly BombSystem _bombSystem;

        private readonly int _panelWidth = 280;
        private readonly int _panelHeight = 200; 
        private readonly int _margin = 20;

        public GameUI(GraphicsDevice graphicsDevice,
                      SpriteBatch spriteBatch,
                      SpriteFont font,
                      Texture2D pixel,
                      Player player,
                      CurrencySystem currency,
                      BombSystem bombSystem)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _font = font;
            _pixel = pixel;
            _player = player;
            _currency = currency;
            _bombSystem = bombSystem;
        }

        public void Draw(GameTime gameTime)
        {
            var vp = _graphicsDevice.Viewport;

            int x = vp.Width - _panelWidth - _margin;
            int y = _margin;

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

           
            var panel = new Rectangle(x, y, _panelWidth, _panelHeight);
            _spriteBatch.Draw(_pixel, panel, Color.Black * 0.45f);

          
            Vector2 livesPos = new(x + 16, y + 16);
            Vector2 coinsPos = new(x + 16, y + 56);
            Vector2 bombBarPos = new(x + 16, y + 106);

            _spriteBatch.DrawString(_font, $"Lives: {_player.Health}", livesPos, Color.Red);
            _spriteBatch.DrawString(_font, $"Coins: {_currency.PlayerCurrency}", coinsPos, Color.Gold);

        
            float current = _bombSystem.CurrentCooldown;
            float max = _bombSystem.Cooldown;
            float readyPercent = MathHelper.Clamp(1f - (current / max), 0f, 1f);

            int barW = 160;
            int barH = 16;
            var barBg = new Rectangle((int)bombBarPos.X, (int)bombBarPos.Y, barW, barH);
            var barFill = new Rectangle((int)bombBarPos.X, (int)bombBarPos.Y, (int)(barW * readyPercent), barH);

            _spriteBatch.Draw(_pixel, barBg, Color.Gray * 0.6f);
            _spriteBatch.Draw(_pixel, barFill, readyPercent >= 1f ? Color.LimeGreen : Color.OrangeRed);

            _spriteBatch.DrawString(
                _font,
                readyPercent >= 1f ? "Bomb: Ready" : $"Bomb: {(current > 0 ? current.ToString("0.0") + "s" : "Ready")}",
                new Vector2(bombBarPos.X + barW + 10, bombBarPos.Y - 2),
                Color.White
            );

       
            Vector2 controlTextPos = new(x + 16, y + _panelHeight - 60);
            _spriteBatch.DrawString(_font,
                "Controls:",
                controlTextPos,
                Color.Cyan);

            _spriteBatch.DrawString(_font,
                "SPACE = Bomb",
                controlTextPos + new Vector2(0, 20),
                Color.White);

            _spriteBatch.DrawString(_font,
                "Q = Tile ability",
                controlTextPos + new Vector2(0, 40),
                Color.White);

            _spriteBatch.DrawString(_font,
                "TAB = Shop",
                controlTextPos + new Vector2(140, 20),
                Color.White);

            _spriteBatch.End();
        }
    }
}