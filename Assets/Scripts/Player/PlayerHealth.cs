using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl
{
    /// <summary>
    /// Gère la barre de détection du joueur qui se remplit quand il est détecté
    /// et diminue progressivement quand il n'est plus détecté
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Paramètres de détection")]
        [SerializeField] private float maxDetection = 100f;
        [SerializeField] private float currentDetection = 0f;

        [Header("Augmentation de détection")]
        [Tooltip("Points de détection gagnés par seconde quand détecté")]
        [SerializeField] private float detectionGainPerSecond = 10f;

        [Header("Diminution de détection")]
        [Tooltip("Points de détection perdus par seconde quand non détecté")]
        [SerializeField] private float detectionLossPerSecond = 5f;
        [Tooltip("Délai avant que la détection commence à diminuer")]
        [SerializeField] private float detectionCooldownDelay = 1f;

        [Header("État")]
        [SerializeField] private bool isDetected = false;
        [SerializeField] private bool canDetect = false;
        [SerializeField] private int detectorCount = 0;

        private float timeSinceLastDetection = 0f;

        // Événements
        public UnityEvent<float, float> OnHealthChanged; // currentDetection, maxDetection
        public UnityEvent OnPlayerDeath; // Appelé quand détection = max
        public UnityEvent<bool> OnDetectionStateChanged;

        // Propriétés publiques
        public float CurrentHealth => currentDetection;
        public float MaxHealth => maxDetection;
        public float HealthPercentage => currentDetection / maxDetection;
        public float DetectionPercentage => currentDetection / maxDetection;
        public bool IsDetected => isDetected;
        public bool IsFullyDetected => currentDetection >= maxDetection;
        public bool IsDead => currentDetection >= maxDetection;
        public int DetectorCount => detectorCount;

        private void Awake()
        {
            currentDetection = 0f;
        }

        private void Update()
        {
            if (!canDetect) return;

            if (isDetected && detectorCount > 0 && !IsFullyDetected)
            {
                // Augmenter la détection (cumulative avec le nombre de détecteurs)
                GainDetection(detectionGainPerSecond * detectorCount * Time.deltaTime);
                timeSinceLastDetection = 0f;
            }
            else if (!isDetected && currentDetection > 0)
            {
                // Diminuer la détection après le délai
                timeSinceLastDetection += Time.deltaTime;

                if (timeSinceLastDetection >= detectionCooldownDelay)
                {
                    LoseDetection(detectionLossPerSecond * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Active ou désactive le système de détection (appelé par GameManager)
        /// </summary>
        public void EnableHealthLoss(bool enable)
        {
            canDetect = enable;

            if (!enable)
            {
                detectorCount = 0;
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

                if (!detected)
                {
                    timeSinceLastDetection = 0f;
                }
            }
        }

        /// <summary>
        /// Augmente la détection du joueur
        /// </summary>
        private void GainDetection(float amount)
        {
            if (IsFullyDetected) return;

            currentDetection += amount;
            currentDetection = Mathf.Min(currentDetection, maxDetection);

            OnHealthChanged?.Invoke(currentDetection, maxDetection);

            if (currentDetection >= maxDetection)
            {
                OnFullyDetected();
            }
        }

        /// <summary>
        /// Diminue la détection du joueur
        /// </summary>
        private void LoseDetection(float amount)
        {
            if (currentDetection <= 0) return;

            currentDetection -= amount;
            currentDetection = Mathf.Max(0, currentDetection);

            OnHealthChanged?.Invoke(currentDetection, maxDetection);
        }

        /// <summary>
        /// Appelé quand la détection atteint le maximum
        /// </summary>
        private void OnFullyDetected()
        {
            OnPlayerDeath?.Invoke();
            isDetected = false;
        }

        /// <summary>
        /// Réinitialise la détection du joueur (appelé après respawn)
        /// </summary>
        public void ResetHealth()
        {
            currentDetection = 0f;
            isDetected = false;
            detectorCount = 0;
            timeSinceLastDetection = 0f;
            OnHealthChanged?.Invoke(currentDetection, maxDetection);
        }

        /// <summary>
        /// Appelé par le système de détection quand le joueur est détecté
        /// </summary>
        public void OnDetectedByEnemy()
        {
            if (canDetect)
            {
                detectorCount++;
                SetDetected(true);
            }
        }

        /// <summary>
        /// Appelé par le système de détection quand le joueur n'est plus détecté
        /// </summary>
        public void OnLostByEnemy()
        {
            detectorCount = Mathf.Max(0, detectorCount - 1);

            if (detectorCount == 0)
            {
                SetDetected(false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            currentDetection = Mathf.Clamp(currentDetection, 0, maxDetection);
        }
#endif
    }
}
