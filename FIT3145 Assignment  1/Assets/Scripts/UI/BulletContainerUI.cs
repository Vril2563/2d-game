using System;
using System.Collections;
using Guns;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BulletContainerUI : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;

        private int maxBullets; // Maximum number of bullets in the UI

        private int currentAmmo;
        private BulletUI[] bullets; // Array to hold bullet UI images

        private Gun playerGun;

        private void Start()
        {
            InitializeBulletUI();
        }

        private void OnEnable()
        {
            if (playerGun == null)
                playerGun = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetPlayerGun().GetComponent<Gun>();
            Gun.onShoot += ShootBullet;
            Gun.onReload += ReloadBullets;
            PlayerController.onChangeGun += WeaponSwap;
        }

        private void OnDisable()
        {
            Gun.onShoot -= ShootBullet;
            Gun.onReload -= ReloadBullets;
            PlayerController.onChangeGun -= WeaponSwap;
        }

        private void InitializeBulletUI()
        {
            StopAllCoroutines();
            if (playerGun == null) 
                playerGun = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetPlayerGun().GetComponent<Gun>();
            
            maxBullets = playerGun.GetMagazineSize();

            bullets = new BulletUI[maxBullets];

            if (bulletPrefab == null)
            {
                Debug.LogError("Bullet prefab not found!");
                return;
            }
            
            for (int i = 0; i < maxBullets; i++)
            {
                GameObject bulletImageObject = Instantiate(bulletPrefab, transform);
                bulletImageObject.SetActive(true);
                bullets[i] = bulletImageObject.GetComponent<BulletUI>();
            }
        }

        private void ShootBullet(int currentAmmo)
        {
            this.currentAmmo = currentAmmo;
            bullets[currentAmmo].LaunchBullet();
        }
        
        private void ReloadBullets(float time)
        {
            StartCoroutine(ReloadBulletsCoroutine(time)); 
        }
        
        private IEnumerator ReloadBulletsCoroutine(float time)
        {
            float reloadTimePerBullet = time / maxBullets;
            int bulletsToReload = maxBullets - currentAmmo;

            for (int i = 0; i < bulletsToReload; i++)
            {
                bullets[currentAmmo + i].ReloadBullet();
                yield return new WaitForSeconds(reloadTimePerBullet);
            }
        }
        
        private void WeaponSwap(GameObject newGun)
        {
            playerGun = newGun.GetComponent<Gun>();
            maxBullets = playerGun.GetMagazineSize();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            InitializeBulletUI();
        }

    }
}