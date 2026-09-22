using UnityEngine;
using TMPro;

/// <summary>
/// Gestionnaire d'affichage des objectifs à l'écran
/// </summary>
public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [Header("Références UI")]
    [Tooltip("GameObject parent contenant tout l'UI de l'objectif")]
    [SerializeField] private GameObject objectivePanel;

    [Tooltip("Texte affichant l'objectif")]
    [SerializeField] private TextMeshProUGUI objectiveText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cacher au démarrage
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(false);
        }
    }

    /// <summary>
    /// Affiche un nouvel objectif
    /// </summary>
    /// <param name="objective">Intitulé de l'objectif</param>
    public void ShowObjective(string objective)
    {
        if (string.IsNullOrEmpty(objective))
        {
            HideObjective();
            return;
        }

        if (objectivePanel == null || objectiveText == null)
        {
            Debug.LogWarning("ObjectiveUI: Panel ou Text non assigné!");
            return;
        }

        // Mettre à jour le texte et afficher
        objectiveText.text = $"Objetivo : {objective}";
        objectivePanel.SetActive(true);
    }

    /// <summary>
    /// Cache l'objectif
    /// </summary>
    public void HideObjective()
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("objectivePanel est null dans HideObjective!");
        }
    }

    /// <summary>
    /// Met à jour l'objectif affiché
    /// </summary>
    /// <param name="objective">Nouvel intitulé de l'objectif</param>
    public void UpdateObjective(string objective)
    {
        if (objectiveText != null && !string.IsNullOrEmpty(objective))
        {
            objectiveText.text = $"Objectif : {objective}";
        }
    }

}
