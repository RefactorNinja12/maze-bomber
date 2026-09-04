using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.CurrencyFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using System.Collections.Generic;

namespace BomberRoyal.Core.Features.AbilityFeature
{
    public class AbilityManager
    {
        private readonly Player _player;
        private readonly CurrencySystem _currency;
        private readonly PlaceTileAbility _placeTileAbility;

        public Dictionary<AbilityType, Ability> Abilities { get; }

        public AbilityManager(
            Player player,
            CurrencySystem currency,
            MazeData maze,
            IEventBus eventBus,
            CollisionChecker collision)
        {
            _player = player;
            _currency = currency;


            _placeTileAbility = new PlaceTileAbility(player, maze, eventBus, collision);

            Abilities = new Dictionary<AbilityType, Ability>
            {
                { AbilityType.Health,       new Ability(AbilityType.Health, 10, 10) },
                { AbilityType.BombCooldown, new Ability(AbilityType.BombCooldown, 10, 10) },
                { AbilityType.BombRange,    new Ability(AbilityType.BombRange, 10, 10) },
                { AbilityType.MoveSpeed,    new Ability(AbilityType.MoveSpeed, 10, 10) },
                { AbilityType.PlaceTile,    new Ability(AbilityType.PlaceTile, 15, 5) }
            };
        }
        public void TryUpgrade(AbilityType type)
        {
            var ability = Abilities[type];
            int cost = ability.GetUpgradeCost();

            if (_currency.PlayerCurrency >= cost && ability.Level < ability.MaxLevel)
            {
                _currency.PlayerCurrency -= cost;
                ability.LevelUp();
                ApplyEffect(ability);
            }
        }

        private void ApplyEffect(Ability ability)
        {
            var bombSystem = _player.BombSystem;
           
            if (bombSystem == null) return;

            switch (ability.Type)
            {
                case AbilityType.Health:
                    _player.Health += 20;
                    break;

                case AbilityType.BombCooldown:
                    bombSystem.DecreaseCooldown(0.9f);
                    break;

                case AbilityType.BombRange:
                    bombSystem.IncreaseRadius(1);
                    break;

                case AbilityType.MoveSpeed:
                    _player.Speed *= 1.1f;
                    break;
            }
        }
        public void ActivateTileAbility()
        {
            _placeTileAbility.TryPlaceTile();
        }
        public void Update(float delta)
        {
            _placeTileAbility.Update(delta);
        }
    }
}
