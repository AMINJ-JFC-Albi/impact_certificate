using UnityEngine;
using Detection;

namespace PlayerControl
{
    /// <summary>
    /// Connecte le système de détection avec le système de santé du joueur
    /// </summary>
    [RequireComponent(typeof(BaseDetection))]
    public class DetectionHealthConnector : MonoBehaviour
    {
        private BaseDetection detectionSystem;
        private PlayerHealth playerHealth;

        private void Awake()
        {
            detectionSystem = GetComponent<BaseDetection>();
        }

        private void Start()
        {
            playerHealth = FindObjectOfType<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogWarning($"{gameObject.name} : Aucun PlayerHealth trouvé dans la scène !");
                return;
            }

            // S'abonner aux événements de détection
            if (detectionSystem != null)
            {
                detectionSystem.OnTargetDetected += OnPlayerDetected;
                detectionSystem.OnTargetLost += OnPlayerLost;
            }
        }

        private void OnDestroy()
        {
            // Se désabonner des événements
            if (detectionSystem != null)
            {
                detectionSystem.OnTargetDetected -= OnPlayerDetected;
                detectionSystem.OnTargetLost -= OnPlayerLost;
            }
        }

        /// <summary>
        /// Appelé quand le système de détection détecte une cible
        /// </summary>
        private void OnPlayerDetected(Transform target)
        {
            if (playerHealth != null)
            {
                playerHealth.OnDetectedByEnemy();
            }
        }

        /// <summary>
        /// Appelé quand le système de détection perd la cible
        /// </summary>
        private void OnPlayerLost()
        {
            if (playerHealth != null)
            {
                playerHealth.OnLostByEnemy();
            }
        }
    }
}
