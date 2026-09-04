using BomberRoyal.Core.Enums;
using BomberRoyal.Core.Features.BoardFeature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BomberRoyal.Core.Features.EnemyFeature
{
    public class EnemyRenderer
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly TextureAtlas _walkAtlas;
        private readonly TextureAtlas _attackAtlas;
        private readonly TextureAtlas _deathAtlas;
        private readonly TextureAtlas _explodingWalkAtlas;
        private readonly TextureAtlas _explodingAttackAtlas;
        private readonly List<EnemyBase> _enemies;
        private readonly Dictionary<EnemyBase, float> _frameTimers = new();
        private readonly Dictionary<EnemyBase, int> _frameIndices = new();
        private readonly Texture2D _pixel;
        public bool ShowHitboxes { get; set; } = true;

        public EnemyRenderer(
            SpriteBatch spriteBatch,
            TextureAtlas walkAtlas,
            TextureAtlas attackAtlas,
            TextureAtlas deathAtlas,
            List<EnemyBase> enemies,
            TextureAtlas explodingWalkAtlas,
            TextureAtlas explodingAttackAtlas
        )
        {
            _spriteBatch = spriteBatch;
            _walkAtlas = walkAtlas;
            _attackAtlas = attackAtlas;
            _deathAtlas = deathAtlas;
            _explodingWalkAtlas = explodingWalkAtlas;
            _explodingAttackAtlas = explodingAttackAtlas;
            _enemies = enemies;

            _pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var e in _enemies)
            {
                if (!e.IsAlive && !e.IsDying)
                    continue;

                if (!_frameTimers.ContainsKey(e))
                {
                    _frameTimers[e] = 0f;
                    _frameIndices[e] = 0;
                }

                bool isExploder = e is ExplodingEnemy;
                bool isDying = e.IsDying;
                bool isAttacking = e.IsAttacking;

                TextureAtlas atlas;
                int frameCount;
                float frameDuration;

                if (isDying)
                {
                    atlas = _deathAtlas;
                    frameCount = 10;
                    frameDuration = 0.15f;
                }
                else if (isAttacking)
                {
                    atlas = isExploder && _explodingAttackAtlas != null
                        ? _explodingAttackAtlas
                        : _attackAtlas;

                    frameCount = isExploder ? 9 : 11;
                    frameDuration = 0.10f;
                }
                else
                {
                    atlas = isExploder && _explodingWalkAtlas != null
                        ? _explodingWalkAtlas
                        : _walkAtlas;

                    frameCount = 6;
                    frameDuration = 0.20f;
                }

                _frameTimers[e] += delta;
                if (_frameTimers[e] >= frameDuration)
                {
                    _frameTimers[e] -= frameDuration;
                    if (isDying)
                        _frameIndices[e] = Math.Min(_frameIndices[e] + 1, frameCount - 1);
                    else
                        _frameIndices[e] = (_frameIndices[e] + 1) % frameCount;
                }

                string prefix = e.Direction switch
                {
                    Direction.Up => e.IsDying ? "death_up_" :
                                    e.IsAttacking ? "attack_up_" : "walk_up_",
                    Direction.Down => e.IsDying ? "death_down_" :
                                      e.IsAttacking ? "attack_down_" : "walk_down_",
                    Direction.Left => e.IsDying ? "death_left_" :
                                      e.IsAttacking ? "attack_left_" : "walk_left_",
                    Direction.Right => e.IsDying ? "death_right_" :
                                       e.IsAttacking ? "attack_right_" : "walk_right_",
                    _ => "walk_down_"
                };

                string regionName = $"{prefix}{_frameIndices[e] + 1}";
                if (!atlas.TryGetRegion(regionName, out var region))
                    continue;

                float renderScale = 64f / e.Width;

                Color tint;
                if (isDying)
                {
                    float fade = 1f - (_frameIndices[e] / (float)(frameCount - 1));
                    tint = new Color(1f, fade, fade, fade);
                }
                else if (isExploder && isAttacking)
                {
                    float pulse = (float)Math.Abs(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10));
                    tint = Color.Lerp(Color.White, Color.Red, pulse);
                }
                else if (isAttacking)
                {
                    tint = Color.Lerp(Color.White, Color.OrangeRed, 0.3f);
                }
                else if (isExploder)
                {
                    tint = Color.Lerp(Color.White, Color.Red,
                        MathHelper.Clamp(e.AttackFlashTimer * 5f, 0f, 1f));
                }
                else
                {
                    tint = Color.Lerp(Color.White, Color.LightGreen,
                        MathHelper.Clamp(e.AttackFlashTimer * 6f, 0f, 1f));
                }

                region.Draw(
                    _spriteBatch,
                    e.Position,
                    tint,
                    0f,
                    new Vector2(region.Width / 2f, region.Height / 2f),
                    new Vector2(renderScale),
                    SpriteEffects.None,
                    0f
                );

                
            }

            _spriteBatch.End();
        }
        // för debugggin av hitboxes
        private void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            spriteBatch.Draw(_pixel, rect, color);
        }
    }
}