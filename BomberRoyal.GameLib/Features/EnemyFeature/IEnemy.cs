using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.EnemyFeature
{
    public interface IEnemy
    {
        Vector2 Position { get; }
        bool IsAlive { get; }
        void Update(float deltaTime);
        void Kill();
    }
}
