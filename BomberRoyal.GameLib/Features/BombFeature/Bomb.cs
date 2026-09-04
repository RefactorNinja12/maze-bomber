using BomberRoyal.Core.Event;
using Microsoft.Xna.Framework;


namespace BomberRoyal.Core.Features.BombFeature
{
    public class Bomb
    {
        private readonly IEventBus _eventBus;
        public Vector2 Position { get; set; }
        public float Timer {  get; private set; }
        public int Radius { get;  set; }
        public bool Exploded { get; private set; } = false;
        public bool IgnorePlayerCollision { get; set; } = true;
        public int Width { get; private set; } = 32;
        public int Height { get; private set; } = 32;


        public Rectangle Hitbox =>
       new Rectangle((int)(Position.X - Width / 2), (int)(Position.Y - Height / 2), Width, Height);

        public Bomb(Vector2 position, IEventBus eventBus, float timer = 2f, int radius = 1)
        {
            _eventBus = eventBus;
            Timer = timer;
            Position = position;
            Radius = radius;
        }
        public void Update(float deltatime)
        {
            if (Exploded) return; 
            Timer -= deltatime;
            if(Timer <= 0)
            {
                Exploded = true;
                _eventBus.Publish(new BombExplodedEvent(this));
            }
        }
    }
}
