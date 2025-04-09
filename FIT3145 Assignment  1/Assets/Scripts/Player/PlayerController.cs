using System;
using System.Collections;
using Bullets;
using Guns;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using VFX;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerController : MonoBehaviour, IDamageable, IKnockbackable, IStatusEffectable
    {
        [Header("References")]
        [SerializeField] private GameObject playerGunObject;
        [SerializeField] private Collider2D playerBulletCollider;
        [SerializeField] private SpriteRenderer playerSprite;
        [SerializeField] private GameObject playerSpriteHolder;
        [SerializeField] private Animator playerAnimator;
        private Color playerDefaultColor;
        public static Action<GameObject> onChangeGun;
        public static Action<Color> onChangeColor;
        private Gun playerGun;
        
        [Header("Audio")]
        [SerializeField] private AudioClip playerHitSound;
        private AudioSource playerAudioSource;

        [Header("Functional Parameters")]
        [SerializeField] bool CanMove = true;
        [SerializeField] bool CanShoot = true;
        [SerializeField] bool CanRoll = true;
        
        [Header("Health Parameters")]   
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private int currentHealth;
        [SerializeField] private float invincibilityDuration = 1f;
        private bool isInvincible = false;
        public static Action<int> onPlayerHit;
        public static Action onPlayerDeath;
        
        
    
        [Header("Movement Parameters")]
        [SerializeField] private float movementSpeed = 5f;

        [Header("Roll Parameters")]
        [SerializeField] private float rollForce = 5f;
        [SerializeField] private float rollFallOff = 3f;
        [SerializeField] private float rollDuration = 0.5f;
        [SerializeField] private float rollFallOffStart = .5f;
        private float rollTimer;

        private PlayerInputActions playerControls;
        private InputAction movementAction;
        private InputAction fireAction;
        private InputAction rollAction;

        private Rigidbody2D rb = new Rigidbody2D();
        private Vector2 movementInput;
    
        private bool isKnockedBack = false;
        
        private Coroutine statusEffectCoroutine;

        private enum PlayerState
        {
            Normal,
            Rolling
        }

        [SerializeField] private PlayerState playerState = PlayerState.Normal;
    
        private void Awake()
        {
            playerControls = new PlayerInputActions();
            rb = GetComponent<Rigidbody2D>();
            if(playerGunObject == null)
                playerGun = GetComponent<Gun>();
            else
                playerGun = playerGunObject.GetComponent<Gun>();
            
            playerDefaultColor = playerSprite.color;
            currentHealth = maxHealth;
            playerAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            onChangeGun?.Invoke(playerGunObject);
            onChangeColor?.Invoke(playerGun.GetChargeColor());
        }

        private void OnEnable()
        {
            movementAction = playerControls.Player.Move;
            movementAction.Enable();
        
            fireAction = playerControls.Player.Fire;
            fireAction.Enable();
        
            rollAction = playerControls.Player.Roll;
            rollAction.Enable();
        }
    
        private void OnDisable()
        {
            movementAction.Disable();
            fireAction.Disable();
            rollAction.Disable();
        }

    
        void Update()
        {

            if (CanMove)
            {
                HandlePlayerMovement();
                HandlePlayerRotation();
            }
            
            if (CanShoot) HandlePlayerShooting();

            if (CanRoll) HandlePlayerRolling();
        }
    
        private void HandlePlayerMovement()
        {
            movementInput = movementAction.ReadValue<Vector2>();
            rb.velocity = movementInput * movementSpeed;
            
            if (rb.velocity != Vector2.zero)
            {
                playerAnimator.SetBool("Running", true);
            }
            else
            {
                playerAnimator.SetBool("Running", false);
            }
        }

        private void HandlePlayerShooting()
        {
            if (playerState == PlayerState.Rolling) return;
            
            if (fireAction.WasPressedThisFrame())
                playerGun.Shoot();
            if(fireAction.WasReleasedThisFrame())
                playerGun.StopShooting();
        }

        private void HandlePlayerRolling()
        {
            if (!rollAction.triggered || playerState == PlayerState.Rolling || rb.velocity.magnitude == 0) return;
            
            StartCoroutine(RollCoroutine());
        }
    
        private IEnumerator RollCoroutine()
        {
            playerAnimator.SetTrigger("Roll");
            
            playerState = PlayerState.Rolling;
            // playerSprite.color = Color.green;
            float startRollForce = rollForce;
            Vector2 rollDirection = movementInput.normalized;
            playerBulletCollider.enabled = false;
            
            // Disable movement while rolling
            CanMove = false;
            while (rollTimer < rollDuration)
            {
                rollTimer += Time.deltaTime;
                rb.velocity = rollDirection * rollForce;
                if (rollTimer >= rollFallOffStart)
                {
                    rollForce = rollFallOff;
                    // playerSprite.color = Color.yellow;
                    playerBulletCollider.enabled = true;
                }
                yield return null;
            }
            // Reset roll force
            rollForce = startRollForce;
            // Reset roll timer
            rollTimer = 0f;
            CanMove = true;
            // playerSprite.color = playerDefaultColor;
            playerState = PlayerState.Normal;
        }

        public void Knockback(Vector2 direction, float initialStrength, float duration)
        {
            if (isKnockedBack) return;

            isKnockedBack = true;
            rb.AddForce(direction * initialStrength, ForceMode2D.Impulse);

            // Apply knockback speed decrease over time
            StartCoroutine(KnockbackWithControl(direction, initialStrength, duration));
        }

        //TODO find a way to cancel out the knockback when moving in opposite direction and holding down another key
        private IEnumerator KnockbackWithControl(Vector2 direction, float initialStrength, float duration)
        {
            float elapsedTime = 0f;
            float minKnockbackStrength = 1f; // Adjust this value as needed

            while (elapsedTime < duration)
            {
                // Calculate the current knockback strength based on lerp progress
                float currentStrength = Mathf.Lerp(initialStrength, minKnockbackStrength, elapsedTime / duration);

                // Apply the knockback force with the blend of player's movement
                Vector2 targetVelocity = direction * currentStrength + movementInput * movementSpeed;
                rb.velocity = targetVelocity;

                elapsedTime += Time.deltaTime;

                // Check if the velocity becomes zero, indicating the player canceled out the knockback
                if (rb.velocity.magnitude < 0.1f)
                {
                    // Exit the coroutine early to end the knockback effect
                    break;
                }

                yield return null;
            }

            // Ensure knockback speed is reset to zero at the end
            rb.velocity = Vector2.zero;

            isKnockedBack = false;
        }
        
        public void AddMaxHealth(int amount)
        {
            maxHealth += amount;
        }

        public void Heal(int amount)
        {
            if (currentHealth + amount > maxHealth) currentHealth = maxHealth;
            else currentHealth += amount;
        }

        public void TakeDamage(int damage)
        {
            if (isInvincible) return;
            currentHealth -= damage;
            onPlayerHit?.Invoke(currentHealth);
//            CameraShake.instance.ShakeCamera(1f, 10f);

            playerAudioSource.PlayOneShot(playerHitSound);
            
            if (currentHealth <= 0) Die();
            else StartCoroutine(InvincibilityCoroutine());
        }

        private void HandlePlayerRotation()
        {
            if (playerGun.GetSpriteFlipped())
            {
                playerSpriteHolder.transform.localRotation = Quaternion.Euler(0f,180f,0f);
            }
            else
            {
                playerSpriteHolder.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            }
        }
        
        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            playerBulletCollider.enabled = false;
            playerSprite.color = Color.red;
            
            yield return new WaitForSeconds(invincibilityDuration);
            
            playerSprite.color = playerDefaultColor;
            playerBulletCollider.enabled = true;
            isInvincible = false;
        }
        
        public void Die()
        {
            onPlayerDeath?.Invoke();
            SceneManager.Instance.ReloadScene();
            Destroy(gameObject);
            Debug.Log("Player is dead");
        }

        public void ApplyStatusEffect(IEnumerator statusEffect)
        {
            if (statusEffectCoroutine != null)
            {
                StopCoroutine(statusEffectCoroutine);
            }
            
            if (statusEffect != null)
                statusEffectCoroutine = StartCoroutine(statusEffect);
        }
        
        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }
        
        public Gun GetPlayerGun()
        {
            if (playerGun == null)
                return playerGunObject.GetComponent<Gun>();
            else 
                return playerGun;
        }
        
        public void PickupGun(GameObject newGun)
        {
            // Destroy the current gun if it exists
            if (playerGunObject != null)
                Destroy(playerGunObject);
            
            playerGunObject = Instantiate(newGun, transform.position, Quaternion.identity);
            playerGunObject.transform.parent = transform;
            playerGun = playerGunObject.GetComponent<Gun>();
            
            onChangeGun?.Invoke(playerGunObject);
            onChangeColor?.Invoke(playerGun.GetChargeColor());
        }

        public void HealPlayer(int i)
        {
            if (currentHealth + i > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += i;
            }

            onPlayerHit?.Invoke(currentHealth);
        }
    }
}
