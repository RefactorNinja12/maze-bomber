using BomberRoyal.Core.Enums;
using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BombFeature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;

namespace BomberRoyal.Core.Features.PlayerFeature
{
    public class Player
    {
        public Direction Direction { get; private set; } = Direction.Down;
        public Vector2 LastMoveDirection { get; private set; } = Vector2.Zero;
        public Vector2 Position { get; private set; }
        private readonly IEventBus _eventBus; 
        public bool IsAlive { get; private set; } = true;
        public bool IsHurt { get; private set; } = false;
        public bool IsDying { get; private set; } = false;

        public float Speed { get; set; } = 150f;
        public int Width { get; set; } = 30;
        public int Height { get; set; } = 30;
        public int Health { get;  set; }

        private float _hurtTimer = 0f;
        private readonly float _hurtDuration = 0.6f;
        private float _deathTimer = 0f;
        private readonly float _deathDuration = 1.2f; 

        private IBombSystem _bombSystem;
        public IBombSystem BombSystem => _bombSystem;



        public Rectangle Hitbox =>
    new(
        (int)(Position.X - Width / 2f),
        (int)(Position.Y - Height / 2f),
        Width, Height
    );

        public Player(Vector2 startPosition, IEventBus eventBus)
        {
            _eventBus = eventBus; 

            Position = startPosition;
            
            Health = 300;
        }
        public void AssignBombSystem(IBombSystem bombSystem)
        {
            _bombSystem = bombSystem;
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive || IsDying) return;

            Health -= amount;
            IsHurt = true;
            _hurtTimer = _hurtDuration;

            if (Health <= 0)
                StartDeath();
        }

        public void Move(Vector2 direction, float deltaTime)
        {
            if (!IsAlive || IsDying) return;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                Position += direction * Speed * deltaTime;
                LastMoveDirection = direction;

                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                    Direction = direction.X > 0 ? Direction.Right : Direction.Left;
                else
                    Direction = direction.Y > 0 ? Direction.Down : Direction.Up;
            }
            else
            {
                LastMoveDirection = Vector2.Zero;
            }
        }

        public void PlaceBomb()
        {
            if (!IsAlive || IsDying) return;
            _bombSystem.TryPlaceBomb(Position);
        }

        public void Update(float deltaTime)
        {
            if (IsHurt)
            {
                _hurtTimer -= deltaTime;
                if (_hurtTimer <= 0f)
                {
                    IsHurt = false;
                    _hurtTimer = 0f;
                    
                }
            }

            if (IsDying)
            {
                _deathTimer -= deltaTime;
                if (_deathTimer <= 0f)
                {
                    IsDying = false;
                    IsAlive = false;
                   
                    _eventBus.Publish(new PlayerDiedEvent(this));
                    
                }
            }
        }

        private void StartDeath()
        {
            if (IsDying) return;

            IsHurt = false;
            IsDying = true;
            _deathTimer = _deathDuration;
            
        }
    }
}