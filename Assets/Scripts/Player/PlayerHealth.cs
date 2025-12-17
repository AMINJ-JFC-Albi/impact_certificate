using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl
{
    /// <summary>
    /// Gère la santé du joueur avec perte de vie progressive pendant la détection
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Paramètres de santé")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Perte de vie en infiltration")]
        [Tooltip("Points de vie perdus par seconde quand détecté")]
        [SerializeField] private float healthLossPerSecond = 5f;

        [Header("État")]
        [SerializeField] private bool isDetected = false;
        [SerializeField] private bool canLoseHealth = false; // Contrôlé par le GameManager

        // Événements
        public UnityEvent<float, float> OnHealthChanged; // currentHealth, maxHealth
        public UnityEvent OnPlayerDeath;
        public UnityEvent<bool> OnDetectionStateChanged; // isDetected

        // Propriétés publiques
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercentage => currentHealth / maxHealth;
        public bool IsDetected => isDetected;
        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // Perdre de la vie seulement si autorisé (mode infiltration) et détecté
            if (canLoseHealth && isDetected && !IsDead)
            {
                LoseHealth(healthLossPerSecond * Time.deltaTime);
            }
        }

        /// <summary>
        /// Active ou désactive la perte de vie (appelé par GameManager)
        /// </summary>
        public void EnableHealthLoss(bool enable)
        {
            canLoseHealth = enable;
            
            // Si on désactive, réinitialiser la détection
            if (!enable)
            {
                SetDetected(false);
            }
        }

        /// <summary>
        /// Définit l'état de détection du joueur
        /// </summary>
        public void SetDetected(bool detected)
        {
            if (isDetected != detected)
            {
                isDetected = detected;
                OnDetectionStateChanged?.Invoke(isDetected);
            }
        }

        /// <summary>
        /// Fait perdre de la vie au joueur
        /// </summary>
        private void LoseHealth(float amount)
        {
            if (IsDead) return;

            currentHealth -= amount;
            currentHealth = Mathf.Max(0, currentHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Appelé quand le joueur meurt
        /// </summary>
        private void Die()
        {
            Debug.Log("Le joueur est mort !");
            OnPlayerDeath?.Invoke();
            
            // Réinitialiser l'état
            canLoseHealth = false;
            isDetected = false;
        }

        /// <summary>
        /// Appelé par le système de détection quand le joueur est détecté
        /// </summary>
        public void OnDetectedByEnemy()
        {
            // Seulement si la perte de vie est activée (mode infiltration)
            if (canLoseHealth)
            {
                SetDetected(true);
            }
        }

        /// <summary>
        /// Appelé par le système de détection quand le joueur n'est plus détecté
        /// </summary>
        public void OnLostByEnemy()
        {
            SetDetected(false);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // S'assurer que la santé actuelle ne dépasse pas la santé max
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
#endif
    }
}
