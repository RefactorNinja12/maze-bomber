using BomberRoyal.Core.Enums;
using BomberRoyal.Core.Features.BoardFeature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BomberRoyal.Core.Features.PlayerFeature
{
    public class PlayerRenderer
    {
        private readonly Player _player;
        private readonly TextureAtlas _walkAtlas;
        private readonly TextureAtlas _idleAtlas;
        private readonly TextureAtlas _hurtAtlas;
        private readonly TextureAtlas _deathAtlas;
        private readonly SpriteBatch _spriteBatch;

        private float _frameTimer = 0f;
        private int _currentFrame = 0;
        private int _frameCount = 6;
        private float _frameDuration = 0.15f;
        private readonly bool _wasMovingLastFrame = false;

        
        private const float TILE_SIZE = 42f;

        public PlayerRenderer(
            Player player,
            TextureAtlas walkAtlas,
            TextureAtlas idleAtlas,
            TextureAtlas hurtAtlas,
            TextureAtlas deathAtlas,
            SpriteBatch spriteBatch)
        {
            _player = player;
            _walkAtlas = walkAtlas;
            _idleAtlas = idleAtlas;
            _hurtAtlas = hurtAtlas;
            _deathAtlas = deathAtlas;
            _spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime)
        {
            if (!_player.IsAlive && !_player.IsDying)
                return;

            bool isMoving = _player.LastMoveDirection != Vector2.Zero;
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameTimer += delta;

            TextureAtlas atlas;
            string prefix;

            if (_player.IsDying)
            {
                atlas = _deathAtlas;
                _frameDuration = 0.1f;
                _frameCount = 10;
                prefix = _player.Direction switch
                {
                    Direction.Up => "death_up_",
                    Direction.Down => "death_down_",
                    Direction.Left => "death_left_",
                    Direction.Right => "death_right_",
                    _ => "death_down_"
                };
            }
            else if (_player.IsHurt)
            {
                atlas = _hurtAtlas;
                _frameDuration = 0.1f;
                _frameCount = 5;
                prefix = _player.Direction switch
                {
                    Direction.Up => "hurt_up_",
                    Direction.Down => "hurt_down_",
                    Direction.Left => "hurt_left_",
                    Direction.Right => "hurt_right_",
                    _ => "hurt_down_"
                };
            }
            else
            {
                atlas = isMoving ? _walkAtlas : _idleAtlas;
                _frameDuration = isMoving ? 0.12f : 0.18f;
                _frameCount = 6;
                prefix = _player.Direction switch
                {
                    Direction.Up => isMoving ? "walk_up_" : "idle_up_",
                    Direction.Down => isMoving ? "walk_down_" : "idle_down_",
                    Direction.Left => isMoving ? "walk_left_" : "idle_left_",
                    Direction.Right => isMoving ? "walk_right_" : "idle_right_",
                    _ => "idle_down_"
                };
            }

            if (_frameTimer >= _frameDuration)
            {
                _frameTimer -= _frameDuration;
                if (_player.IsDying)
                    _currentFrame = Math.Min(_currentFrame + 1, _frameCount - 1);
                else
                    _currentFrame = (_currentFrame + 1) % _frameCount;
            }

            string regionName = $"{prefix}{_currentFrame + 1}";
            if (!atlas.TryGetRegion(regionName, out var region))
                return;

            Color tint = _player.IsDying
                ? new Color(1f, 1f, 1f, 1f - (_currentFrame / (float)_frameCount))
                : _player.IsHurt ? Color.Red : Color.White;

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            region.Draw(
                _spriteBatch,
                _player.Position,
                tint,
                0f,
                new Vector2(region.Width / 2f, region.Height / 2),
                new Vector2(2f),
                SpriteEffects.None,
                0f
            );


          


            _spriteBatch.End();
        }
        // debug
        private void DrawHitbox(Player player)
        {
            var rect = player.Hitbox;
            var pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Red });
            _spriteBatch.Draw(pixel, rect, Color.Red * 0.4f);
        }

    }
}