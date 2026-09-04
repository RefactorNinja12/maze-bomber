using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Shared
{
    public class RandomGenerator
    {
        private readonly Random rnd = new();
        public int GenerateMazeRandom(int start, int end)
        {
            if (end <= start)
                return start;
            return rnd.Next(start, end);
        }
    }
}
