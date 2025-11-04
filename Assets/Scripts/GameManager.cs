using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire central du jeu qui gère les modes d'interaction.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Liste statique de tous les objets interactifs enregistrés
    private static List<InteractableObject> registeredInteractables = new List<InteractableObject>();

    // Flag global pour savoir si les interactions sont bloquées
    private static bool interactionsEnabled = true;
    public static bool InteractionsEnabled => interactionsEnabled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Enregistre un objet interactif dans la liste globale.
    /// </summary>
    public static void RegisterInteractable(InteractableObject interactable)
    {
        if (!registeredInteractables.Contains(interactable))
        {
            registeredInteractables.Add(interactable);
        }
    }

    /// <summary>
    /// Désenregistre un objet interactif de la liste globale.
    /// </summary>
    public static void UnregisterInteractable(InteractableObject interactable)
    {
        registeredInteractables.Remove(interactable);
    }

    /// <summary>
    /// Active ou désactive toutes les interactions dans le jeu.
    /// </summary>
    /// <param name="active">True pour activer les interactions, False pour les désactiver</param>
    public void ChangeInteractionMode(bool active)
    {

        interactionsEnabled = active;

        // Nettoyer les références null
        registeredInteractables.RemoveAll(item => item == null);

        // Mettre à jour tous les objets interactifs enregistrés
        foreach (var interactable in registeredInteractables)
        {
            if (interactable != null)
            {
                interactable.SetActive(active);
            }
        }

        // Si on désactive les interactions, forcer le masquage du tooltip actuellement affiché
        if (!active && ToolTipsManager.Instance != null)
        {
            ToolTipsManager.Instance.HideToolTip();
        }

    }
}
