using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BomberRoyal.Core.Features.AbilityFeature
{
    public class AbilityMenu
    {
        private readonly AbilityManager _abilityManager;
        private bool _isOpen = false;
        private float _resumeDelay = 0f;

        private float _keyDelay = 0f;
        private const float KEY_COOLDOWN = 1.5f; 

        private readonly List<AbilityType> _abilityOrder = new()
        {
            AbilityType.Health,
            AbilityType.BombCooldown,
            AbilityType.BombRange,
            AbilityType.MoveSpeed
        };

        public bool IsOpen => _isOpen;
        public bool IsPaused => _isOpen || _resumeDelay > 0f;

        public AbilityMenu(AbilityManager abilityManager)
        {
            _abilityManager = abilityManager;
        }

        public void Toggle()
        {
            if (_isOpen)
            {
                _isOpen = false;
                _resumeDelay = 3f;
            }
            else if (_resumeDelay <= 0f)
            {
                _isOpen = true;
            }
        }

        public void Update(float delta)
        {
            if (_resumeDelay > 0f)
                _resumeDelay -= delta;

            if (_keyDelay > 0f)
                _keyDelay -= delta;

            if (!_isOpen)
                return;

            if (_keyDelay > 0f)
                return;

            var state = Keyboard.GetState();

            for (int i = 0; i < _abilityOrder.Count; i++)
            {
                Keys key = Keys.D1 + i;
                if (state.IsKeyDown(key))
                {
                    _abilityManager.TryUpgrade(_abilityOrder[i]);
                    _keyDelay = KEY_COOLDOWN;
                    break;
                }
            }
        }

        public IEnumerable<(Rectangle rect, string label, Ability ability, int index)> GetCards()
        {
            for (int i = 0; i < _abilityOrder.Count; i++)
            {
                var type = _abilityOrder[i];
                var ability = _abilityManager.Abilities[type];
                var rect = new Rectangle(100 + i * 180, 300, 160, 200);
                yield return (rect, type.ToString(), ability, i + 1);
            }
        }
    }
}