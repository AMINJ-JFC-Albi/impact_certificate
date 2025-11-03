using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Composant qui gère si un objet peut être interagi ou non.
/// Utilise un système de compteur pour gérer plusieurs sources de blocage.
/// </summary>
public class CanInteractWithIt : MonoBehaviour
{
    private int canInteract = 0;

    private void Awake()
    {
        // S'enregistrer automatiquement dans le GameManager
        GameManager.RegisterInteractable(this);
    }

    private void OnDestroy()
    {
        // Se désenregistrer quand l'objet est détruit
        GameManager.UnregisterInteractable(this);
    }

    /// <summary>
    /// Vérifie si l'objet peut être interagi.
    /// </summary>
    /// <returns>True si l'objet est interactif, False sinon</returns>
    public bool CanInteract()
    {
        // Vérifier que le compteur est à 0 (pas de blocage actif)
        if (canInteract > 0)
            return false;

        // Vérifier qu'aucun élément UI ne bloque le raycast
        if (Mouse.current != null && EventSystem.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false; // UI bloque l'interaction
            }
        }

        return true;
    }

    /// <summary>
    /// Active ou désactive l'interactivité de l'objet.
    /// </summary>
    /// <param name="active">True pour activer, False pour désactiver</param>
    public void SetActive(bool active)
    {
        if (active)
        {
            canInteract--;
            if (canInteract < 0)
            {
                canInteract = 0;
            }
        }
        else
        {
            canInteract++;
        }

    }

    /// <summary>
    /// Réinitialise le compteur à 0 (force l'activation).
    /// </summary>
    public void ResetCounter()
    {
        canInteract = 0;
    }

    /// <summary>
    /// Récupère la valeur actuelle du compteur (pour debug).
    /// </summary>
    public int GetCounter()
    {
        return canInteract;
    }
}
