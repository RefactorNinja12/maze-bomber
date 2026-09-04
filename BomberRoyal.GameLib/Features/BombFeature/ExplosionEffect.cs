using Microsoft.Xna.Framework;

namespace BomberRoyal.Core.Features.BombFeature
{
    public class ExplosionEffect
    {
        public Vector2 Position { get; }
        public ExplosionType Type { get; }
        public bool Active { get; private set; } = true;

        private float _timer = 0f;
        private const float FrameDuration = 0.08f; 
        private const int FrameCount = 10;          
        private int _currentFrame = 0;

        public ExplosionEffect(Vector2 pos, ExplosionType type)
        {
            Position = pos;
            Type = type;
        }

        public void Update(float delta)
        {
            _timer += delta;
            if (_timer >= FrameDuration)
            {
                _timer -= FrameDuration;
                _currentFrame++;
                if (_currentFrame >= FrameCount)
                    Active = false;
            }
        }

        public int GetFrame() => _currentFrame + 1;
    }
}
