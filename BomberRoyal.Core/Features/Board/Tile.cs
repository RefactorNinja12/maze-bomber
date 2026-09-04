using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.Board
{
    public class Tile
    {
        public int Size { get; private set; }
        public bool IsDestructible { get; private set; }


        public int X { get; private set; }
        public int Y { get; private set; }

        public Tile(int x, int y, int size, bool isDestructible)
        {
            X = x;
            Y = y;
            Size = size;
            IsDestructible = isDestructible;
        }

        public bool IsWalkable() => IsDestructible;

        public void Destroy() { if (IsDestructible) IsDestructible = false; }
    }
}
