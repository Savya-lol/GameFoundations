using UnityEngine;

namespace Darkmatter.Core.Abilities
{
    public interface IAbility
    {
        bool IsActive { get; }
        void Activate();
        void Deactivate();
        void Tick();
    }

}
