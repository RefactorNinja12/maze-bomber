using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.PlayerFeature
{
    public class PlayerController
    {
        private Player _player; 

        public PlayerController(Player player)
        {
            _player = player;
        }
        public void Update(float deltaTime)
        {
            var input = InputManager.Instance;
            Vector2 direction = Vector2.Zero;

            if (input.IsKeyDown(IsKeyDown.W)) direction.Y -= 1;
            if (input.IsKeyDown(IsKeyDown.S)) direction.Y -= 1;
            if (input.IsKeyDown(IsKeyDown.A)) direction.Y -= 1;
            if (input.IsKeyDown(IsKeyDown.D)) direction.Y -= 1;
            _player.Move(direction, deltaTime);
            if (input.IsKeyDown(IsKeyDown.Space))
            {
                _player.PlaceBomb();
            }
        }
    }
}
