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
        private PlayerHealth lastDetectedPlayerHealth; // Stocker la dernière cible détectée

        private void Awake()
        {
            detectionSystem = GetComponent<BaseDetection>();
        }

        private void Start()
        {
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
            // Récupérer PlayerHealth de la cible détectée
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                lastDetectedPlayerHealth = playerHealth; // Sauvegarder la référence
                playerHealth.OnDetectedByEnemy();
            }
        }

        /// <summary>
        /// Appelé quand le système de détection perd la cible
        /// </summary>
        private void OnPlayerLost()
        {
            // Utiliser la dernière cible détectée sauvegardée
            if (lastDetectedPlayerHealth != null)
            {
                lastDetectedPlayerHealth.OnLostByEnemy();
                lastDetectedPlayerHealth = null; // Nettoyer la référence
            }
        }
    }
}
