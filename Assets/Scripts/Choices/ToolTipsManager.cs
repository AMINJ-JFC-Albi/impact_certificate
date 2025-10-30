using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Gère l'affichage et le comportement des infobulles dans le jeu.
/// </summary>
public class ToolTipsManager : MonoBehaviour
{
    public static ToolTipsManager Instance;

    [Header("Éléments UI")]
    [SerializeField]
    private TextMeshProUGUI textComponent;
    private GameObject transformToolTips;



    /// <summary>
    /// Initialise l'instance singleton
    /// </summary>
    private void Awake()
    {
        // Vérifie si une instance de ToolTipManager existe déjà
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        transformToolTips = transform.GetChild(0).gameObject;

    }

    private void Update()
    {
        if (transformToolTips.activeSelf)
        {
            Move();
        }
    }

    /// <summary>
    /// Met à jour la position de l'infobulle en fonction de la position de la souris.
    /// </summary>
    private void Move()
    {
        // Déplace l'infobulle en fonction de la position de la souris
        transformToolTips.transform.position = Input.mousePosition;
    }

    /// <summary>
    /// Affiche l'infobulle avec le texte spécifié.
    /// </summary>
    /// <param name="text">Le texte à afficher dans l'infobulle.</param>

    public void ShowToolTip(string text)
    {
        // Affiche l'infobulle avec le texte spécifié
        textComponent.text = text;
        Move();
        transformToolTips.SetActive(true);

    }

    /// <summary>
    /// Cache l'infobulle.
    /// </summary>
    public void HideToolTip()
    {
        // Cache l'infobulle
        transformToolTips.SetActive(false);

    }


}
