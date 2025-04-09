using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");
        Debug.Log("Health pickup collided with " + other.name);
        if (other.GetComponent<PlayerController>())
        {
            other.GetComponent<PlayerController>().HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}
