using Interfaces;
using UnityEngine;

namespace Bullets.Basic
{
    public class TerraBullet : Bullet
    {
        protected override void SpecialCollisionEffects(Collider2D other)
        {
            if (other.GetComponent<IKnockbackable>() != null) 
                other.GetComponent<IKnockbackable>().Knockback(rb.velocity.normalized, 
                    bulletData.knockbackStrength, bulletData.knockbackDuration);
        }
    }
}
