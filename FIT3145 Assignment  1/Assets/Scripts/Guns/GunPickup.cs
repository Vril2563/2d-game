using System;
using Player;
using UnityEngine;

namespace Guns
{
    public class GunPickup : MonoBehaviour
    {
        [SerializeField] private GameObject gunPrefab;
        private PlayerController playerController;

        private void Start()
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerController.PickupGun(gunPrefab);
                Destroy(this.gameObject);
            }
        }
    }
}
