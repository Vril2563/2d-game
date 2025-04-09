using System;
using System.Collections;
using System.Collections.Generic;
using Bullets;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Guns
{
    public abstract class Gun : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] protected AudioClip shootSound;
        [FormerlySerializedAs("etartReloadSound")] [SerializeField] protected AudioClip startReloadSound;
        [SerializeField] protected AudioClip endReloadSound;
        protected AudioSource gunAudioSource;
        
        [Header("Gun Parameters")]
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected Transform firePoint;
        [SerializeField] protected Transform aimTransform;
        [SerializeField] protected GameObject gunSprite;
        [SerializeField] protected bool isPlayerWeapon;

        [Header("Ammo Parameters")]
        [SerializeField] protected bool infiniteAmmo = false;
        [SerializeField] protected int magazineSize = 5;
        [SerializeField] protected int currentAmmo;
        [SerializeField] protected float reloadTime = 2f;
        public static Action<int> onShoot;
        public static Action<float> onReload;

        [Header("Shoot Parameters")]
        [SerializeField] private float shootCooldown = 1f;
        [SerializeField] protected bool isAutomatic = false;
        [SerializeField, Range(0f, 90f)] protected float spread = 0;
        protected bool readyToShoot = true;
        private bool shouldShoot = false;
        private bool shooting = false;

        protected Bullet bullet;
        protected Transform target;
        
        protected bool shouldAim = true;

        private float angle;
        private GameObject lastBulletInstance;
        private bool reloading = false;
        
        private float rotationSnap = .7f;
        private bool spriteFlipped = false;

        private void Awake()
        {
            if (aimTransform == null)
                aimTransform = transform.Find("Aim");
            bullet = bulletPrefab.GetComponent<Bullet>();
            gunAudioSource = GetComponent<AudioSource>();
            currentAmmo = magazineSize;
        }

        void Update()
        {
            if (shouldAim) HandleAiming();
        }

        private void HandleAiming()
        {
            Vector3 aimDirection = (ChooseTarget() - aimTransform.position).normalized;
            angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aimTransform.eulerAngles = new Vector3(0, 0, angle);
            
            if (aimTransform.rotation.z > rotationSnap || aimTransform.rotation.z < -rotationSnap)
            {
                gunSprite.transform.localRotation = Quaternion.Euler(180f,0f,0f);
                spriteFlipped = true;
            }
            else
            {
                gunSprite.transform.localRotation = Quaternion.Euler(0f,0f,0f);
                spriteFlipped = false;
            }
        }
    
        public void Shoot()
        {
            if (!readyToShoot || reloading) return;
            if (currentAmmo <= 0 && !infiniteAmmo)
            {
                StartCoroutine(Reload());
                return;
            }
            shouldShoot = true;
            if (isAutomatic)
            {
                StartCoroutine(ShootAutomatic());
            }
            else
            {
                SpawnBullet();
                StartCoroutine(ShootCooldown());
            }
        }

        public void StopShooting()
        {
            shouldShoot = false;
        }

        private IEnumerator ShootAutomatic()
        {
            if (shooting) yield break;
            
            shooting = true;
            while (shouldShoot && (currentAmmo > 0 || infiniteAmmo))
            {
                SpawnBullet();
                yield return new WaitForSeconds(shootCooldown);
            }
            shooting = false;
        }

        private IEnumerator ShootCooldown()
        {
            readyToShoot = false;
            yield return new WaitForSeconds(shootCooldown);
            readyToShoot = true;
        }

        protected void SpawnBullet()
        {
            currentAmmo--;
            if (isPlayerWeapon)
                onShoot?.Invoke(currentAmmo);
            
            // Play shoot sound
            gunAudioSource.PlayOneShot(shootSound);
            
            Vector3 aimDirection = (ChooseTarget() - aimTransform.position).normalized;

            // Apply random spread to the aim direction
            float randomSpread = Random.Range(-spread, spread);
            float spreadAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + randomSpread;
            aimDirection = Quaternion.Euler(0, 0, spreadAngle) * Vector3.right;

            Vector3 velocity = aimDirection.normalized * bullet.bulletData.speed;

            lastBulletInstance = BulletManager.Instance.InstantiateBullet(bulletPrefab,
                firePoint.position, Quaternion.identity, velocity, 
                isPlayerWeapon ? Bullet.Origin.player : Bullet.Origin.enemy);

            if (isPlayerWeapon)
                lastBulletInstance.GetComponent<Bullet>().LastBulletShot(true);
        }

        private IEnumerator Reload()
        {
            // TODO reload animation and sound
            reloading = true;
            
            if (isPlayerWeapon)
                onReload?.Invoke(reloadTime);
            gunAudioSource.PlayOneShot(startReloadSound);
            
            yield return new WaitForSeconds(reloadTime);
            
            gunAudioSource.PlayOneShot(endReloadSound);
            reloading = false;
            currentAmmo = magazineSize;
        }


        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            return mouseWorldPosition;
        }
        
        public void SetTarget(Transform _target)
        {
            if (isPlayerWeapon)
            {
                Debug.LogError("Cannot set target for player weapon (It is set to mouse position)");
                return;
            }
            target = _target;
        }
        
        public void SetShouldAim(bool _shouldAim)
        {
            shouldAim = _shouldAim;
        }

        private Vector3 ChooseTarget()
        {
            if (target == null)
                return GetMouseWorldPosition();

            if (isPlayerWeapon) return 
                GetMouseWorldPosition();
            
                return target.position;
        }
        
        public int GetCurrentAmmo()
        {
            return currentAmmo;
        }
        
        public bool GetSpriteFlipped()
        {
            return spriteFlipped;
        }
        
        public Color GetChargeColor()
        {
            if (bullet == null)
            {
                Debug.LogError("Bullet is null");
                return Color.white;
            }
            return bullet.GetBulletColor();
        }
        
        public int GetMagazineSize()
        {
            return magazineSize;
        }
    }
}
