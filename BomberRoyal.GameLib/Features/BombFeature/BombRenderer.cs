using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BoardFeature;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace BomberRoyal.Core.Features.BombFeature
{
    public class BombRenderer
    {
        private readonly IEventBus _eventBus;
        private TextureAtlas _bombAtlas;
        private SpriteBatch _spriteBatch;
        private List<Bomb> _bombs = new();

        private const int FrameCount = 4;
        private const float FrameDuration = 0.1f;

        private float _frameTimer = 0f;
        private int _currentFrame = 0;

        private readonly Dictionary<Bomb, float> _frameTimers = new();
        private readonly Dictionary<Bomb, int> _frameIndices = new();

        public BombRenderer(TextureAtlas bombAtlas, SpriteBatch spriteBatch, IEventBus eventBus)
        {
            _bombAtlas = bombAtlas;
            _eventBus = eventBus; 
            _spriteBatch = spriteBatch;
            _eventBus.Subscribe<BombPlacedEvent>(OnBombPlaced);
            _eventBus.Subscribe<BombExplodedEvent>(OnBombExploded);
        }
        private void OnBombPlaced(BombPlacedEvent e)
        {
            var bomb = e.Bomb;
            if (!_bombs.Contains(bomb))
                _bombs.Add(bomb);
        }

        private void OnBombExploded(BombExplodedEvent e)
        {
            var bomb = e.Bomb;
            _bombs.Remove(bomb);
            _frameTimers.Remove(bomb);
            _frameIndices.Remove(bomb);
        }
        public void Draw(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameTimer += delta;

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var bomb in _bombs)
            {
                if (bomb.Exploded) continue;

                if (!_frameTimers.ContainsKey(bomb))
                {
                    _frameTimers[bomb] = 0f;
                    _frameIndices[bomb] = 0;
                }

                _frameTimers[bomb] += delta;

                if (_frameTimers[bomb] >= FrameDuration)
                {
                    _frameTimers[bomb] -= FrameDuration;
                    _frameIndices[bomb] = (_frameIndices[bomb] + 1) % FrameCount;
                }

                string regionName = $"slime_bomb_{_frameIndices[bomb] + 1}";

                if (!_bombAtlas.TryGetRegion(regionName, out var region)) continue;


                region.Draw(
                    _spriteBatch,
                    bomb.Position,
                    Color.White,
                    0f,
                    new Vector2(region.Width / 2f, region.Height / 2f),
                    new Vector2(0.40f),
                    SpriteEffects.None,
                    0f
                    );

            }
            _spriteBatch.End();
        }
    }
}
