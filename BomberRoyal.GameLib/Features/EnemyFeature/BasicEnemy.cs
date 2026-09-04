using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.PathfindingFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System;

namespace BomberRoyal.Core.Features.EnemyFeature
{
    public class BasicEnemy : EnemyBase
    {
        public BasicEnemy(Vector2 startPos, CollisionChecker c, Player p, IEventBus eventBus, EnemyFlowFieldService flowField)
            : base(startPos, c, p, eventBus, flowField)
        {
            Speed = 30f;
            Health = 100f;
            Damage = 25;
            AttackCooldown = 1.0f;
        }
        protected override bool TryAttack()
        {
            if (_currentCooldown > 0)
                return false;

            if (_collisionChecker.CollidesWithPlayer(Hitbox))
            {
                
                _player.TakeDamage(Damage);
                IsAttacking = true;
                _attackAnimTimer = 0.5f;
                _currentCooldown = AttackCooldown;
                AttackFlashTimer = 0.5f;
                return true;
            }

            return false;
        }
    }
}