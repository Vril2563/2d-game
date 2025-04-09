using System.Collections;
using System.Collections.Generic;
using Bullets;
using Enemies;
using Interfaces;
using UnityEngine;

public class GlaciesBullet : Bullet
{
    [SerializeField] private float freezeDuration = 10f;

    protected override void SpecialCollisionEffects(Collider2D other)
    {
        // if (other.GetComponent<IStatusEffectable>() != null)
        //    other.GetComponent<IStatusEffectable>().ApplyStatusEffect(Freeze(other));
    }

    private IEnumerator Freeze(Collider2D other)
    {
        Entity entity = null;
        
        if (other.GetComponent<Entity>())
        {
            entity = other.GetComponent<Entity>();
        }
        else
        {
            Debug.Log("No entity found on " + other.name);
            yield break;
        }
        
        if (entity == null) yield break;
        
        entity.SetCanMove(false);
        entity.SetCanAttack(false);

        if (other.GetComponent<Enemy>())
        {
            other.GetComponent<Enemy>().SetCanAim(false);
        }

        Debug.Log("FreezeStatusEffectStarting");
        yield return new WaitForSeconds(freezeDuration); 
        Debug.Log("FreezeStatusEffectStopping");
        entity.SetCanMove(true);
        entity.SetCanAttack(true);
        
        if (other.GetComponent<Enemy>())
        {
            other.GetComponent<Enemy>().SetCanAim(true);
        }
    }
}
