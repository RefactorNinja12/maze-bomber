using Microsoft.Xna.Framework;


namespace BomberRoyal.Core.Features.CurrencyFeature
{
    public class CurrencyPickup
    {
        public Vector2 Position { get;}
        public int Amount { get;}
        public bool Collected { get; private set;  }
        private readonly int _size = 24;

        public CurrencyPickup(Vector2 postion, int amount)
        {
            Position = postion;
            Amount = amount;
        }
        public Rectangle Hitbox =>
           new((int)(Position.X - _size / 2), (int)(Position.Y - _size / 2), _size, _size);

        public void Collect()
        {
            Collected = true;
        }



    }
}
