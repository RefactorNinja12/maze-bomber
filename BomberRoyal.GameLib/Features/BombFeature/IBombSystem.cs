using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.BombFeature
{
    public interface IBombSystem
    {
        
            IReadOnlyList<Bomb> ActiveBombs { get; }
            float Cooldown { get; }
            float CurrentCooldown { get; }
            int DefaultRadius { get; }

            void TryPlaceBomb(Vector2 playerPosition);
            void Update(float delta);
            void IncreaseRadius(int amount);
            void DecreaseCooldown(float factor);
        
    }
}
