using BomberRoyal.Core.Features.CurrencyFeature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberRoyal.Core.Features.AbilityFeature
{
    public class AbilityMenuRenderer
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _pixel;
        private readonly SpriteFont _font;
        private readonly AbilityMenu _menu;
        private readonly CurrencySystem _currency;

        public AbilityMenuRenderer(
            SpriteBatch spriteBatch,
            Texture2D pixel,
            SpriteFont font,
            AbilityMenu menu,
            CurrencySystem currency)
        {
            _spriteBatch = spriteBatch;
            _pixel = pixel;
            _font = font;
            _menu = menu;
            _currency = currency;
        }

        public void Draw()
        {
            if (!_menu.IsOpen)
                return;

            _spriteBatch.Begin();

          
            _spriteBatch.Draw(_pixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.7f);

          
            _spriteBatch.DrawString(
                _font,
                $"Currency: {_currency.PlayerCurrency}",
                new Vector2(40, 40),
                Color.Gold
            );

          
            foreach (var (rect, label, ability, index) in _menu.GetCards())
            {
                Color bg = Color.DarkSlateGray;
                _spriteBatch.Draw(_pixel, rect, bg);

             
                var titlePos = new Vector2(rect.X + 20, rect.Y + 20);
                _spriteBatch.DrawString(_font, $"{index}. {label}", titlePos, Color.White);

             
                var levelText = $"Lvl {ability.Level}/{ability.MaxLevel}";
                _spriteBatch.DrawString(_font, levelText, new Vector2(rect.X + 20, rect.Y + 60), Color.Yellow);

                var costText = $"Cost: {ability.GetUpgradeCost()}";
                _spriteBatch.DrawString(_font, costText, new Vector2(rect.X + 20, rect.Y + 100), Color.Gold);

        
                _spriteBatch.DrawString(
                    _font,
                    $"Press {index} to Upgrade",
                    new Vector2(rect.X + 20, rect.Y + 150),
                    Color.LightGray
                );
            }

            _spriteBatch.End();
        }
    }
}