using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace BomberRoyal.Core.Features.CurrencyFeature
{
    public class CurrencyRenderer
    {
        private readonly Texture2D _coinTexture;
        private readonly CurrencySystem _system;

        public CurrencyRenderer(Texture2D coinTexture, CurrencySystem system)
        {
            _coinTexture = coinTexture;
            _system = system;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var pickup in _system.Pickups.Where(p => !p.Collected))
            {
                
                Rectangle destRect = new Rectangle(
                    (int)(pickup.Position.X - _coinTexture.Width / 2),
                    (int)(pickup.Position.Y - _coinTexture.Height / 2),
                    _coinTexture.Width,
                    _coinTexture.Height
                );

                spriteBatch.Draw(
                    _coinTexture,
                    pickup.Position,
                    null,
                    Color.White,
                    0f,
                    new Vector2(_coinTexture.Width / 2f, _coinTexture.Height / 2f),
                    42f / _coinTexture.Width,
                    SpriteEffects.None,
                    0f
                );
            }

            spriteBatch.End();
        }
    }
}