using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

/// <summary>
/// Gestionnaire central du jeu qui gère les modes d'interaction.
/// Par défaut, tous les objets sont NON-interactifs.
/// Seuls les objets dans la liste "allowedInteractables" peuvent être interagis.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Objets interactifs autorisés")]
    [Tooltip("Liste des objets qui peuvent être interagis. Tous les autres sont bloqués.")]
    [SerializeField] private List<InteractableObject> allowedInteractables = new List<InteractableObject>();

    [Header("Mode Infiltration")]
    [Tooltip("Active le mode infiltration au démarrage du jeu")]
    [SerializeField] private bool infiltrationMode = false;

    // Événement statique pour le changement de mode infiltration
    public static System.Action<bool> OnInfiltrationModeChanged;

    // Propriété publique pour accéder au mode infiltration
    public static bool IsInInfiltrationMode { get; private set; } = false;

    // Liste statique de tous les objets interactifs enregistrés
    private static List<InteractableObject> registeredInteractables = new List<InteractableObject>();

    // Flag global pour savoir si les interactions sont bloquées
    private static bool interactionsEnabled = true;
    public static bool InteractionsEnabled => interactionsEnabled;

    private PlayerHealth playerHealth;

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

    private void Start()
    {
        // Appliquer la liste d'objets autorisés au démarrage
        UpdateAllowedInteractables();

        // Trouver le PlayerHealth
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        // Initialiser le mode infiltration
        SetInfiltrationMode(infiltrationMode);
    }

    private void Update()
    {
        // Mettre à jour le mode infiltration si changé dans l'Inspector
        if (IsInInfiltrationMode != infiltrationMode)
        {
            SetInfiltrationMode(infiltrationMode);
        }
    }

    /// <summary>
    /// Active ou désactive le mode infiltration par script
    /// </summary>
    public void SetInfiltrationMode(bool active)
    {
        infiltrationMode = active;
        IsInInfiltrationMode = active;
        
        // Déclencher l'événement
        OnInfiltrationModeChanged?.Invoke(active);
        
        // Activer/désactiver la perte de vie du joueur
        if (playerHealth != null)
        {
            playerHealth.EnableHealthLoss(active);
        }
    }

    /// <summary>
    /// Met à jour l'état de tous les objets interactifs selon la liste autorisée
    /// </summary>
    public void UpdateAllowedInteractables()
    {
        registeredInteractables.RemoveAll(item => item == null);
        allowedInteractables.RemoveAll(item => item == null);

        foreach (var interactable in registeredInteractables)
        {
            if (interactable == null) continue;

            if (allowedInteractables.Contains(interactable))
            {
                // Objet autorisé : réinitialiser et activer
                interactable.ResetCounter();
                interactable.UpdateOutlineState();
            }
            else
            {
                // Objet non autorisé : réinitialiser et bloquer
                interactable.ResetCounter();
                interactable.SetActive(false);
                interactable.UpdateOutlineState();
            }
        }
    }

    /// <summary>
    /// Ajoute un objet à la liste des objets autorisés
    /// </summary>
    public void AllowInteractable(InteractableObject interactable)
    {
        if (interactable != null && !allowedInteractables.Contains(interactable))
        {
            allowedInteractables.Add(interactable);
            interactable.ResetCounter();
            interactable.UpdateOutlineState();
            Debug.Log($"Objet {interactable.gameObject.name} ajouté aux objets autorisés");
        }
    }

    /// <summary>
    /// Retire un objet de la liste des objets autorisés
    /// </summary>
    public void DisallowInteractable(InteractableObject interactable)
    {
        if (interactable != null && allowedInteractables.Contains(interactable))
        {
            allowedInteractables.Remove(interactable);
            interactable.SetActive(false); // Le désactiver (met aussi à jour l'outline)
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
