using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PlayerControl
{
    /// <summary>
    /// Gère l'affichage de la barre de vie du joueur dans l'interface
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Couleurs de la barre")]
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private float warningThreshold = 0.5f;
        [SerializeField] private float criticalThreshold = 0.25f;

        [Header("Animation")]
        [SerializeField] private bool animateHealthChange = true;
        [SerializeField] private float animationSpeed = 3f;

        private PlayerHealth playerHealth;
        private float targetFillAmount;
        private float currentFillAmount;

        private void Start()
        {
            playerHealth = FindObjectOfType<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogError("HealthBarUI : Aucun PlayerHealth trouvé dans la scène !");
                return;
            }

            // S'abonner aux événements
            playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            GameManager.OnInfiltrationModeChanged += OnInfiltrationModeChanged;

            // Initialiser l'affichage
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            targetFillAmount = playerHealth.HealthPercentage;
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
            // Animer le changement de la barre de vie
            if (animateHealthChange && healthBarFill != null)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
                healthBarFill.fillAmount = currentFillAmount;
            }
        }

        /// <summary>
        /// Appelé quand la santé change
        /// </summary>
        private void OnHealthChanged(float currentHealth, float maxHealth)
        {
            UpdateHealthBar(currentHealth, maxHealth);
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
        /// Met à jour l'affichage de la barre de vie
        /// </summary>
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
            targetFillAmount = healthPercentage;

            // Mettre à jour la couleur selon le pourcentage et le texte
            healthBarFill.color = GetHealthColor(healthPercentage);
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";

        }

        /// <summary>
        /// Retourne la couleur appropriée selon le pourcentage de santé
        /// </summary>
        private Color GetHealthColor(float healthPercentage)
        {
            if (healthPercentage <= criticalThreshold)
            {
                return criticalColor;
            }
            else if (healthPercentage <= warningThreshold)
            {
                float t = (healthPercentage - criticalThreshold) / (warningThreshold - criticalThreshold);
                return Color.Lerp(criticalColor, warningColor, t);
            }
            else
            {
                float t = (healthPercentage - warningThreshold) / (1f - warningThreshold);
                return Color.Lerp(warningColor, healthyColor, t);
            }
        }

        /// <summary>
        /// Affiche ou masque manuellement la barre de vie
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

    }
}
