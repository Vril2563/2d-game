using System.Collections;
using Interfaces;
using UnityEngine;

namespace Bullets.Basic
{
    public class IgnitaBullet : Bullet
    {
        [SerializeField] private float igniteDuration = 1f;
        [SerializeField] private int igniteDamage = 1;
        
        protected override void SpecialCollisionEffects(Collider2D other)
        {
            if (other.GetComponent<IStatusEffectable>() != null)
               other.GetComponent<IStatusEffectable>().ApplyStatusEffect(Ignite(other));
        }
        
        private IEnumerator Ignite(Collider2D other)
        {
            //TODO add ignite effect
            
            yield return new WaitForSeconds(igniteDuration);
            
            other.GetComponent<IDamageable>().TakeDamage(igniteDamage);
            Debug.Log("Ignite damage dealt");
            //TODO remove ignite effect
        }
    }
}
