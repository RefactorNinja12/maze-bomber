using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.PlayerFeature
{
    public class Player
    {
        public Vector2 Postition { get; private set; }
        public int Health = 100;
        public bool IsAlive { get; private set; } = true;
        public string Name { get; set; }
        public float Speed { get; set; } = 150f;
        public int TileSize { get; set; } = 32;

        public Player(Vector2 startPosition)
        {
            Postition = startPosition;
        }
        public void Move(Vector2 direction, float deltaTime)
        {
            if (!IsAlive)
            {
                return; 
            }
            if(direction != Vector2.Zero)
            {
                direction.Normalizer();
                Postition += direction * Speed * deltaTime;
            }
        }
        public void PlaceBomb()
        {
            if (!IsAlive)
            {
                return;
            }
            Console.WriteLine("Placed a bomb Tillfälligt"); 
        }

    }
}
