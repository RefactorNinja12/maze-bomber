using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.AbilityFeature
{
    public class Ability
    {
        public AbilityType Type { get;  }
        public int Level { get; private set; } = 1; 
        public int MaxLevel { get; }
        public int BaseCost { get; }

        public Ability(AbilityType type, int baseCost, int maxLevel)
        {
            Type = type;
            BaseCost = baseCost;
            MaxLevel = maxLevel;
        }
        public int GetUpgradeCost() => BaseCost * Level;
        public void LevelUp()
        {
            if(Level < MaxLevel)
            {
                Level++; 
            }
        }


    }
}
