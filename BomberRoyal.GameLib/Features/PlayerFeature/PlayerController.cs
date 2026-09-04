using BomberRoyal.Core.Features.AbilityFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace BomberRoyal.Core.Features.PlayerFeature
{
    public class PlayerController
    {
        private readonly Player _player;
        private readonly InputManager _input;
        private readonly CollisionChecker _collision;
        private readonly AbilityManager _abilityManager;

        public PlayerController(Player player, InputManager inputManager, CollisionChecker collisioncheck, AbilityManager abilityManager)
        {
            _abilityManager = abilityManager;
            _player = player;
            _input = inputManager;
            _collision = collisioncheck;
        }
        public void Update(float deltaTime)
        {

            Vector2 direction = Vector2.Zero;

            if (_input.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (_input.IsKeyDown(Keys.S)) direction.Y += 1;
            if (_input.IsKeyDown(Keys.A)) direction.X -= 1;
            if (_input.IsKeyDown(Keys.D)) direction.X += 1;
            if (_input.isKeyPressed(Keys.Space))
            {
                
                _player.PlaceBomb();
            }

            if (_input.isKeyPressed(Keys.Q))
                _abilityManager.ActivateTileAbility();

            
            _abilityManager.Update(deltaTime);
            if (direction == Vector2.Zero) return;

            direction.Normalize();

          
            Vector2 movement = direction * _player.Speed * deltaTime;
            Vector2 nextPos = _player.Position + movement;

            
            Rectangle nextHitbox = new Rectangle(
                (int)(nextPos.X - _player.Width / 2),
                (int)(nextPos.Y - _player.Height / 2),
                _player.Width,
                _player.Height
            );

            if (_collision.IsBlocked(nextHitbox, _player))
            {
                Console.WriteLine("Player movement blocked");
            }
            else
            {
                Console.WriteLine("Player can move");
                _player.Move(direction, deltaTime);
            }
        }
    }
}
