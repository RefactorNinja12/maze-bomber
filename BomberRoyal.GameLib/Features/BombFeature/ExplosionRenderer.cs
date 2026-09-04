using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BoardFeature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BomberRoyal.Core.Features.BombFeature
{
    public class ExplosionRenderer
    {
        private readonly IEventBus _eventBus;
        private readonly SpriteBatch _spriteBatch;
        private readonly TextureAtlas _atlas;
        private readonly List<ExplosionEffect> _explosions = new();
        public ExplosionRenderer(SpriteBatch spriteBatch, TextureAtlas atlas, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _spriteBatch = spriteBatch;
            _atlas = atlas;
            _eventBus.Subscribe<BombExplodedEvent>(OnBombExploded);
        }

        private void OnBombExploded(BombExplodedEvent e)
        {
            var bomb = e.Bomb;
            int tileSize = 42;

           
            _explosions.Add(new ExplosionEffect(bomb.Position, ExplosionType.Center));

           
            for (int i = 1; i <= bomb.Radius; i++)
            {
                bool isEnd = i == bomb.Radius;

                _explosions.Add(new ExplosionEffect(
                    new Vector2(bomb.Position.X + tileSize * i, bomb.Position.Y),
                    isEnd ? ExplosionType.EndRight : ExplosionType.MiddleRight));

                _explosions.Add(new ExplosionEffect(
                    new Vector2(bomb.Position.X - tileSize * i, bomb.Position.Y),
                    isEnd ? ExplosionType.EndLeft : ExplosionType.MiddleLeft));

                _explosions.Add(new ExplosionEffect(
                    new Vector2(bomb.Position.X, bomb.Position.Y + tileSize * i),
                    isEnd ? ExplosionType.EndDown : ExplosionType.MiddleDown));

                _explosions.Add(new ExplosionEffect(
                    new Vector2(bomb.Position.X, bomb.Position.Y - tileSize * i),
                    isEnd ? ExplosionType.EndUp : ExplosionType.MiddleUp));
            }

            
        }

        public void Update(float delta)
        {
            for (int i = _explosions.Count - 1; i >= 0; i--)
            {
                _explosions[i].Update(delta);
                if (!_explosions[i].Active)
                    _explosions.RemoveAt(i);
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var e in _explosions)
            {
               
                string regionName = $"explosion_{e.GetFrame()}";

                if (!_atlas.TryGetRegion(regionName, out var region))
                    continue;

                float scale = 42f / region.Width;

                region.Draw(
                    _spriteBatch,
                    e.Position,
                    Color.White,
                    0f,
                    new Vector2(region.Width / 2f, region.Height / 2f),
                    new Vector2(scale),
                    SpriteEffects.None,
                    0f
                );
            }

            _spriteBatch.End();
        }
    }
}