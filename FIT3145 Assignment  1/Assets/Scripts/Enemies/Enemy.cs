using System;
using System.Collections;
using System.Collections.Generic;
using Guns;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemies
{
    public abstract class Enemy : Entity, IDamageable, IKnockbackable, IStatusEffectable
    {
        [Header("References")]
        [SerializeField] protected Gun enemyGun;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        [Header("Enemy Attack Stats")] 
        [SerializeField] protected bool melee = false;
        [SerializeField] protected float minAttackFrequency = 7f;
        [SerializeField] protected float maxAttackFrequency = 10f;
        [SerializeField] protected float executionChance = 90;
        [SerializeField] protected float attackDuration = 1f;
        private bool attacking = false;
        private float nextAttackTime = 0f;
        
        [Header("Enemy Fov Stats")]
        [SerializeField] private float viewRadius = 5f;
        [SerializeField] private float callFriendsRadius = 20f;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask obstructionMask;
        
        [Header("Audio")]
        [SerializeField] protected AudioClip[] hurtSounds;
        protected AudioSource enemyAudioSource;


        [Header("Functional Options")]
        [SerializeField] private bool sawPlayer;
        [SerializeField] private bool canSeePlayer;

        protected Rigidbody2D rb;
        protected Mover mover;
        protected GameObject player;
        protected Coroutine statusEffectCoroutine;
        protected Transform target;
        protected Color defaultColor;

        protected void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            enemyAudioSource = GetComponent<AudioSource>();
        }

        protected new void Start()
        {
            base.Start();
            
            rb.isKinematic = true;

            enemyGun.SetTarget(GameObject.FindWithTag("Player").transform);
            currentHealth = maxHealth;
            target = player.transform;
            
            sawPlayer = false;
            canSeePlayer = false;

            enemyGun.SetShouldAim(false);
            
            defaultColor = spriteRenderer.color;
            
            StartCoroutine(FOVroutine());
        }

        protected void Update()
        {
            if (player == null) return;
            
            
            if (!sawPlayer) return;
            
            if (canMove) ChaseTarget();
            else mover.Cancel();

            if (canAttack && canSeePlayer) AttackTimer();
        }

        private void AttackTimer()
        {
            // Check if it's time to attack
            if (canAttack && Time.time >= nextAttackTime)
            {
                // Determine if the attack should be executed based on the chance
                if (Random.value <= (executionChance / 100f))
                {
                    StartCoroutine(Attack());
                }
                
                // Set the next attack time with a random delay
                nextAttackTime = Time.time + Random.Range(minAttackFrequency, maxAttackFrequency);
            }
        }

        public void TakeDamage(int damage)
        {
            if (!sawPlayer)
            {
                CallFriends();
                sawPlayer = true;
                enemyGun.SetShouldAim(true);
            }
            
          
            currentHealth -= damage;
            enemyAudioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
            StartCoroutine(DamageFlash());
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            mover.StopTheCoroutine();
            Destroy(gameObject);
        }

        public void Knockback(Vector2 direction, float initialStrength, float duration)
        {
            rb.isKinematic = false;
            rb.AddForce(direction * initialStrength, ForceMode2D.Impulse);
            StartCoroutine(KnockbackTimer(duration));
        }

        private IEnumerator KnockbackTimer(float duration)
        {
            yield return new WaitForSeconds(duration);
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        private void ChaseTarget()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= range && canSeePlayer)
            {
                mover.Cancel();
                return;
            }
            
            
            mover.MoveTo(target.position, currentMoveSpeed);
        }

        protected IEnumerator Attack()
        {
            if (attacking) yield break;
            
            attacking = true;
            enemyGun.Shoot();
            yield return new WaitForSeconds(attackDuration);
            enemyGun.StopShooting();
            attacking = false;
        }
        
        public void ApplyStatusEffect(IEnumerator statusEffect)
        {
            if (statusEffect == null) return;
            if (statusEffectCoroutine != null)
            {
                StopCoroutine(statusEffectCoroutine);
            }
            
            statusEffectCoroutine = StartCoroutine(statusEffect);
        }

        
        private IEnumerator FOVroutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.7f);
            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private IEnumerator DamageFlash()
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = defaultColor;
        }

        private void FieldOfViewCheck()
        {
            if (player == null)
            {
                StopAllCoroutines();
                return;
            }
            Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
            
                
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        if (!sawPlayer)
                        {
                            sawPlayer = true;
                            CallFriends();
                        }
                        enemyGun.SetShouldAim(true);
                        canSeePlayer = true;
                        
                    }
                    else
                        canSeePlayer = false;
            }
            else if (canSeePlayer)
                canSeePlayer = false;
        }
        
        public void SetAttackPlayer(bool attack)
        {
            canAttack = attack;
            sawPlayer = attack;
            enemyGun.SetShouldAim(attack);
        }

        private void CallFriends()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, callFriendsRadius);

            foreach (Collider2D other in hitColliders)
            {
                if (other.GetComponent<Enemy>())
                {
                    other.GetComponent<Enemy>().SetAttackPlayer(true);
                }
            }
        }
        
        public void SetCanAim(bool aim)
        {
            enemyGun.SetShouldAim(aim);
        }
        
        // Draw the range of the enemy in the editor for debugging
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, callFriendsRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            if (canSeePlayer && target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized * viewRadius);
            }
        }
    }
}