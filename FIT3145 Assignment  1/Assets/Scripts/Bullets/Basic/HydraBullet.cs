using System.Collections;
using Interfaces;
using UnityEngine;

namespace Bullets.Basic
{
    public class HydraBullet : Bullet
    {
        [SerializeField] private float slowDuration = 2f;
        [SerializeField] private float slowStrengthDivision = 2f;
        
        protected override void SpecialCollisionEffects(Collider2D other)
        {
        
            if (other.GetComponent<IStatusEffectable>() != null)
            {
                Debug.Log("ApplyingSlowStatusEffect: " + other.name);
                other.GetComponent<IStatusEffectable>().ApplyStatusEffect(SlowStatusEffect(other));
            }
        }
    
        private IEnumerator SlowStatusEffect(Collider2D other)
        {
            Entity entity;
        
            if (other.GetComponent<Entity>())
            {
                entity = other.GetComponent<Entity>();
            }
            else
            {
                Debug.Log("No entity found on " + other.name);
                yield break;
            }
        
            float originialSpeed = entity.GetMaxMoveSpeed();
            float currentSpeed = entity.GetCurrentMoveSpeed();
        
            float targetSpeed = originialSpeed / slowStrengthDivision;
        
            entity.SetCurrentMoveSpeed(targetSpeed);
            yield return new WaitForSeconds(slowDuration);
            Debug.Log("SlowStatusEffectStopping: " + entity.GetMaxMoveSpeed());
            entity.SetCurrentMoveSpeed(originialSpeed);
        }
    }
}
