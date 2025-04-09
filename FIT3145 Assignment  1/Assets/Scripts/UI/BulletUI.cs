using UnityEngine;

namespace UI
{
    public class BulletUI : MonoBehaviour
    {
        [SerializeField] private GameObject fullBulletPrefab;
        [SerializeField] private GameObject fullBullet;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float maxAngularVelocity; // Maximum angular velocity for rotation

        private bool shot = false;
        private Rigidbody2D rb;

        public void LaunchBullet()
        {
            rb = fullBullet.GetComponent<Rigidbody2D>();
            if (shot) return;
            shot = true;
        
            rb.isKinematic = false;
            rb.simulated = true;
        
            // Calculate a random direction vector with more upwards movement
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1f)).normalized;
            float randomAngularVelocity = UnityEngine.Random.Range(-maxAngularVelocity, maxAngularVelocity);

            rb.AddForce(bulletSpeed * randomDirection, ForceMode2D.Impulse);
            rb.angularVelocity = randomAngularVelocity;
        
            Destroy(fullBullet, 5f);
        }

        public void ReloadBullet()
        {
            if (!shot) return;
            shot = false;
            fullBullet = Instantiate(fullBulletPrefab, transform.position, Quaternion.identity, transform);
            rb = fullBullet.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.simulated = false;
        }
    }
}