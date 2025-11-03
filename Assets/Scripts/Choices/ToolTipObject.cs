
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

    private CanInteractWithIt canInteractWithIt;

    private void Awake()
    {
        canInteractWithIt = GetComponent<CanInteractWithIt>();
    }

    /// <summary>
    /// Affiche l'infobulle lorsque la souris entre sur l'objet.
    /// </summary>
    private void OnMouseEnter()
    {
        // Vérifier si l'objet peut être interagi avant d'afficher le tooltip
        if (canInteractWithIt != null && !canInteractWithIt.CanInteract())
        {
            return;
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
