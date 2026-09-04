
using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.PathfindingFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System;

namespace BomberRoyal.Core.Features.EnemyFeature
{
    public class ExplodingEnemy : EnemyBase
    {
        private bool _isExploding = false;
        private float _explodeTimer = 0f;
        private readonly float _explodeDelay = 2.5f;
        private readonly int _explodeRadius = 2;

        public ExplodingEnemy(Vector2 startPos, CollisionChecker c, Player p, IEventBus eventBus, EnemyFlowFieldService flowField)
            : base(startPos, c, p, eventBus, flowField)
        {
            Speed = 35f;
            Health = 80f;
            Damage = 100;
            AttackCooldown = 1.0f;
        }
        public override void Update(float deltaTime)
        {
            if (_isExploding)
            {
                _explodeTimer -= deltaTime;
                AttackFlashTimer = (float)Math.Abs(Math.Sin(_explodeTimer * 8f));
                if (_explodeTimer <= 0f)
                    Explode();
                return;
            }

            base.Update(deltaTime);
        }

        protected override bool TryAttack()
        {
            if (_isExploding || _currentCooldown > 0)
                return false;

            if (_collisionChecker.CollidesWithPlayer(Hitbox))
            {
                BeginSelfDestruct("collided with Player");
                return true;
            }

            return false;
        }

        protected override void OnPathEnd()
        {
           
            BeginSelfDestruct("blocked by wall");
        }
        private void BeginSelfDestruct(string reason)
        {
            if (_isExploding) return;
            _isExploding = true;
            _explodeTimer = _explodeDelay;
            IsAttacking = true;
            _attackAnimTimer = _explodeDelay;
            _currentCooldown = AttackCooldown;
            AttackFlashTimer = _explodeDelay;
        }
        private void Explode()
        {
            var fakeBomb = new Bomb(Position, _eventBus, 0f, _explodeRadius);
            _eventBus.Publish(new BombExplodedEvent(fakeBomb));
            Kill();
        }

        public override void Reset(Vector2 pos)
        {
            base.Reset(pos);
            _isExploding = false;
            _explodeTimer = 0f;
        }
    }
}