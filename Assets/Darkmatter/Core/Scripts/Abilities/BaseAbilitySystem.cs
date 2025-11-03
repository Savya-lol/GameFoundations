using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;

namespace Darkmatter.Core.Abilities
{
    public abstract class BaseAbilitySystem : ITickable
    {
        private readonly IEnumerable<IAbility> _abilities;
        public BaseAbilitySystem(IEnumerable<IAbility> abilities)
        {
            _abilities = abilities;
        }
        public void Tick()
        {
            foreach (var ability in _abilities)
            {
                if (ability.IsActive)
                    ability.Tick();
            }
        }

        public void Tick(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
