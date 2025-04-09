using System.Collections;
using System.Collections.Generic;
using Bullets;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    
    private List<GameObject> bullets = new List<GameObject>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public GameObject InstantiateBullet(GameObject bulletPrefab, Vector2 position, Quaternion rotation, 
        Vector2 velocity, Bullet.Origin origin)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, position, rotation);
        bulletInstance.GetComponent<Rigidbody2D>().velocity = velocity;
        bulletInstance.GetComponent<Bullet>().SetBulletOrigin(origin);
        bullets.Add(bulletInstance);
        bulletInstance.transform.parent = transform;
        return bulletInstance;
    }
}
