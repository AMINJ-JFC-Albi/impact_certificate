using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PlayerControl
{
    /// <summary>
    /// Gère l'affichage de la barre de détection du joueur dans l'interface
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Couleurs de la barre (détection)")]
        [SerializeField] private Color safeColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;
        [Tooltip("Seuil à partir duquel la couleur passe en warning (50%)")]
        [SerializeField] private float warningThreshold = 0.5f;
        [Tooltip("Seuil à partir duquel la couleur passe en danger (75%)")]
        [SerializeField] private float dangerThreshold = 0.75f;

        [Header("Animation")]
        [SerializeField] private bool animateHealthChange = true;
        [SerializeField] private float animationSpeed = 3f;

        private PlayerHealth playerHealth;
        private float targetFillAmount;
        private float currentFillAmount;

        private void Start()
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogError("HealthBarUI : Aucun PlayerHealth trouvé dans la scène !");
                return;
            }

            // S'abonner aux événements
            playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            GameManager.OnInfiltrationModeChanged += OnInfiltrationModeChanged;

            // Initialiser l'affichage (barre vide au départ)
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            targetFillAmount = playerHealth.DetectionPercentage;
            currentFillAmount = targetFillAmount;

            // Afficher selon l'état actuel du mode infiltration
            gameObject.SetActive(GameManager.IsInInfiltrationMode);
        }

        private void OnDestroy()
        {
            // Se désabonner des événements
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
            }

            GameManager.OnInfiltrationModeChanged -= OnInfiltrationModeChanged;
        }

        private void Update()
        {
            // Animer le changement de la barre de détection
            if (animateHealthChange && healthBarFill != null)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
                healthBarFill.fillAmount = currentFillAmount;
            }
        }

        /// <summary>
        /// Appelé quand la détection change
        /// </summary>
        private void OnHealthChanged(float currentDetection, float maxDetection)
        {
            UpdateHealthBar(currentDetection, maxDetection);
        }


        /// <summary>
        /// Appelé quand le mode infiltration change
        /// </summary>
        private void OnInfiltrationModeChanged(bool isInInfiltrationMode)
        {
            // Afficher ou masquer la barre selon le mode infiltration
            gameObject.SetActive(isInInfiltrationMode);
        }

        /// <summary>
        /// Met à jour l'affichage de la barre de détection
        /// </summary>
        private void UpdateHealthBar(float currentDetection, float maxDetection)
        {
            float detectionPercentage = currentDetection / maxDetection;
            targetFillAmount = detectionPercentage;

            // Mettre à jour la couleur selon le pourcentage de détection
            healthBarFill.color = GetDetectionColor(detectionPercentage);

            // Afficher le pourcentage de détection
            int percentDisplay = Mathf.RoundToInt(detectionPercentage * 100f);
            healthText.text = $"Detección: {percentDisplay}%";
        }

        /// <summary>
        /// Retourne la couleur appropriée selon le pourcentage de détection
        /// Plus la détection est haute, plus c'est dangereux (rouge)
        /// </summary>
        private Color GetDetectionColor(float detectionPercentage)
        {
            if (detectionPercentage >= dangerThreshold)
            {
                return dangerColor;
            }
            else if (detectionPercentage >= warningThreshold)
            {
                float t = (detectionPercentage - warningThreshold) / (dangerThreshold - warningThreshold);
                return Color.Lerp(warningColor, dangerColor, t);
            }
            else
            {
                float t = detectionPercentage / warningThreshold;
                return Color.Lerp(safeColor, warningColor, t);
            }
        }

        /// <summary>
        /// Affiche ou masque manuellement la barre de détection
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

    }
}
