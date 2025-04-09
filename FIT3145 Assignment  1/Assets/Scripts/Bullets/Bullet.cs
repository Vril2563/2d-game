using System;
using Enemies;
using Interfaces;
using Scriptable_Objects;
using UnityEngine;

namespace Bullets
{
    public abstract class Bullet : MonoBehaviour
    {
        public GameObject bulletCombineEffect;
        public GameObject bulletHitEffect;
        public AudioClip bulletDestroySound;
        public ScriptableBullet bulletData;
        public SpriteRenderer spriteRenderer;
        public Collider2D bulletCollider;
        private bool lastBulletShot = false;
        protected Rigidbody2D rb;
        protected Collider2D col;
        protected AudioSource audioSource;

        public enum Origin
        {
            player,
            enemy,
            trap
        }
        
        public Origin origin;

        private void Awake()
        {
            if (bulletCollider == null)
                bulletCollider = GetComponent<Collider2D>();
            
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            gameObject.layer = 7;
            audioSource = GetComponent<AudioSource>();
        }
        
        private void Start()
        {
            Destroy(gameObject, bulletData.lifetime);
        }

        private void Update()
        {
            // Align the bullet's rotation with its velocity
            if (rb.velocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            
        }

        public void SetBulletOrigin(Origin _origin)
        {
            origin = _origin;
        }
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollision(other);
        }

        private void HandleCollision(Collider2D other)
        {
            if (other.GetComponent<IDamageable>() != null)
            {
                HandleCollisionDamageable(other);
            }
            else if (other.GetComponent<Bullet>() != null)
                HandleCollisionBullet(other);
            else
            {
                DestroyBullet();
            }
               
        }

        private void DestroyBullet()
        {
            GameObject bulletHitEffectInstance = Instantiate(bulletHitEffect, transform.position, Quaternion.identity);
            var main = bulletHitEffectInstance.GetComponent<ParticleSystem>().main;
            main.startColor = bulletData.bulletColor;
            
            bulletCollider.enabled = false;
            spriteRenderer.enabled = false;
            rb.velocity = Vector2.zero;
            
            audioSource.PlayOneShot(bulletDestroySound);
            Destroy(gameObject, 1f); 
        }

        private void HandleCollisionDamageable(Collider2D other)
        {
            if (origin == Origin.trap)
            {
                other.GetComponent<IDamageable>().TakeDamage(bulletData.damage);
                SpecialCollisionEffects(other);
                DestroyBullet();
            }
            
            if (origin == Origin.player)
            {
                if (other.CompareTag("PlayerHitbox")) return;
                
                other.GetComponent<IDamageable>().TakeDamage(bulletData.damage);
                SpecialCollisionEffects(other);
                DestroyBullet();
            }

            if (origin == Origin.enemy)
            {
                if (other.GetComponent<Enemy>() || other.CompareTag("EnemyHitbox"))
                {
                    Debug.Log("Enemy to enemy collision");
                    return;
                }
                
                other.GetComponent<IDamageable>().TakeDamage(bulletData.damage);
                SpecialCollisionEffects(other);
                DestroyBullet();
            }
        }
        
        private void HandleCollisionBullet(Collider2D other)
        {
            BulletTypes.BulletType otherBulletType = other.GetComponent<Bullet>().bulletData.bulletType;
            Bullet.Origin otherBulletOrigin = other.GetComponent<Bullet>().origin;
            
            if (otherBulletOrigin == origin) return;
            
            if (origin == Origin.player && otherBulletOrigin == Origin.player)
            {
                if (!lastBulletShot) return;

                // We destroy the other bullet to avoid double instancing
                Destroy(other.gameObject);
                
                if (otherBulletType == bulletData.bulletType)
                {
                    MultiplyBullet();
                }
                else
                    CombineBullets(otherBulletType);
            }
            else if (origin == Origin.player && otherBulletOrigin == Origin.enemy)
            {
                // We destroy the other bullet to avoid double instancing
                Destroy(other.gameObject);
                
                if (otherBulletType == bulletData.bulletType)
                {
                    MultiplyBullet();
                }
                else
                    CombineBullets(otherBulletType);
            }
        }

        private void CombineBullets(BulletTypes.BulletType otherBulletType)
        {
            // Bullet combo creates new type of bullet
            GameObject newBullet = bulletData.bulletTypes.GetBulletCombo(bulletData.bulletType, otherBulletType);
            
            // Disable collider so that the new bullet doesn't collide with the old one
            col.enabled = false;
            if (newBullet != null)
            {
                BulletManager.Instance.InstantiateBullet(newBullet, transform.position, transform.rotation, rb.velocity,
                    origin);
                
                // Spawn bullet combine effect
                GameObject bulletCombineEffectInstance = Instantiate(bulletCombineEffect, transform.position, Quaternion.identity);
                var main = bulletCombineEffectInstance.GetComponent<ParticleSystem>().main;
                main.startColor = newBullet.GetComponent<Bullet>().bulletData.bulletColor;
            }
            else // if the bullet combo doesnt exist, keep the original bullet
            {
                col.enabled = true;
                return;
            }
            
            // Default bullet destruction
            DestroyBullet();
        }

        private void MultiplyBullet()
        {
            // TODO - THIS IS BUGGED AND SPAWNS INFINITE BULLETS

            //return; // TODO - remove this when fixed
            
            GameObject bulletPrefab = gameObject;
            
            // Bullet multiplies into three bullets in a shotgun pattern
            Vector2 velocity = rb.velocity;
            
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            float angleOffset = 15f;
            float distanceOffset = 0.5f;
            
            Vector2 velocity1 = Quaternion.AngleAxis(angleOffset, Vector3.forward) * velocity;
            Vector2 velocity2 = Quaternion.AngleAxis(-angleOffset, Vector3.forward) * velocity;
            
            Vector2 position1 = (Vector2) transform.position + velocity1.normalized * distanceOffset;
            Vector2 position2 = (Vector2) transform.position + velocity2.normalized * distanceOffset;

            BulletManager.Instance.InstantiateBullet(bulletPrefab, position1, transform.rotation, velocity1, origin);
            BulletManager.Instance.InstantiateBullet(bulletPrefab, position2, transform.rotation, velocity2, origin);
        }
        
        public void LastBulletShot(bool _lastBulletShot)
        {
            lastBulletShot = _lastBulletShot;
        }
        
        public Color GetBulletColor()
        {
            if (bulletData == null) return Color.white;
            return bulletData.bulletColor;
        }
        
        protected abstract void SpecialCollisionEffects(Collider2D other);
    }
}
