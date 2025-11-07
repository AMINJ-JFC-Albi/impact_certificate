
using UnityEngine;

/// <summary>
/// Gère l'affichage des infobulles pour un objet 3D.
/// Nécessite un Collider sur l'objet.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ToolTipObject : MonoBehaviour
{
    [Header("Paramètres de l'infobulle")]
    public string text; // Texte à afficher

    private InteractableObject interactableObject;

    private void Awake()
    {
        interactableObject = GetComponent<InteractableObject>();
    }

    /// <summary>
    /// Affiche l'infobulle lorsque la souris entre sur l'objet.
    /// </summary>
    private void OnMouseEnter()
    {
        // Vérifier si l'objet peut être interagi avant d'afficher le tooltip
        if (interactableObject != null && !interactableObject.CanInteract())
        {
            Debug.Log($"{gameObject.name}: Tooltip bloqué - CanInteract() = false");
            return;
        }

        if (interactableObject == null)
        {
            Debug.LogWarning($"{gameObject.name}: Aucun InteractableObject trouvé sur cet objet!");
        }

        if (ToolTipsManager.Instance != null)
        {
            ToolTipsManager.Instance.ShowToolTip(text);
        }
        else
        {
            Debug.LogError("ToolTipsManager.Instance est null!");
        }
    }

    /// <summary>
    /// Cache l'infobulle lorsque la souris quitte l'objet.
    /// </summary>
    private void OnMouseExit()
    {
        if (ToolTipsManager.Instance != null)
        {
            ToolTipsManager.Instance.HideToolTip();
        }
    }
}
