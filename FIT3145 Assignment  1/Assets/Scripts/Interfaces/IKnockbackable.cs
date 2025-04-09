using UnityEngine;

namespace Interfaces
{
    public interface IKnockbackable
    {
        void Knockback(Vector2 direction, float force, float duration);
    }
}
