using UnityEngine;
using Player;
using UnityEngine.UI;

namespace UI
{
    public class HeartContainerUI : MonoBehaviour
    {
        [Header("Heart Sprites")]
        [SerializeField] private Sprite fullHeart;
        [SerializeField] private Sprite halfHeart;
        [SerializeField] private Sprite emptyHeart;

        private int maxHeartContainers = 0;
        private int currentHealth;
        private Image[] hearts;

        private PlayerController playerController;

        private void Start()
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            maxHeartContainers = Mathf.CeilToInt(playerController.GetMaxHealth() / 2f); // Each container is for 2 health
            currentHealth = playerController.GetCurrentHealth();

            // Instantiate heart containers
            hearts = new Image[maxHeartContainers];
            for (int i = 0; i < maxHeartContainers; i++)
            {
                GameObject heartContainerObject = new GameObject("HeartContainer" + i);
                heartContainerObject.transform.SetParent(transform);
                hearts[i] = heartContainerObject.AddComponent<Image>();
            }

            foreach (var heart in hearts)
            {
                heart.sprite = fullHeart;
            }
        }

        private void OnEnable()
        {
            PlayerController.onPlayerHit += UpdateHearts;
        }

        private void OnDisable()
        {
            PlayerController.onPlayerHit -= UpdateHearts;
        }

        private void UpdateHearts(int newHealth)
        {
            currentHealth = newHealth;
            int fullHearts = currentHealth / 2;
            int halfHeartRemainder = currentHealth % 2;

            for (int i = 0; i < maxHeartContainers; i++)
            {
                if (i < fullHearts)
                {
                    hearts[i].sprite = fullHeart;
                }
                else if (i == fullHearts && halfHeartRemainder == 1)
                {
                    hearts[i].sprite = halfHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }
            }
        }
    }
}
