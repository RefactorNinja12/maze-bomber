using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.CurrencyFeature
{
    public class CurrencySystem
    {
        private readonly List<CurrencyPickup> _pickups = new();
        private readonly Player _player; 
        
        public int PlayerCurrency { get;  set; }

        public IReadOnlyList<CurrencyPickup> Pickups => _pickups;

        public CurrencySystem(Player player)
        {
            _player = player;
        }

        public void SpawnPickup(Vector2 position, int amount)
        {
            _pickups.Add(new CurrencyPickup(position, amount));
        }
        public void Update()
        {
            for(int i = _pickups.Count - 1; i >= 0; i--)
            {
                var p = _pickups[i];
                if (!p.Collected && p.Hitbox.Intersects(_player.Hitbox))
                {
                    p.Collect();
                    PlayerCurrency += p.Amount;
                }
            }
        }

        public void ClearCollected()
        {
            _pickups.RemoveAll(p => p.Collected);
        }
    }
}
