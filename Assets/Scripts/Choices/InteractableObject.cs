using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Type d'interaction pour l'objet interactif
/// </summary>
public enum InteractionType
{
    MoveToObject,
    ClickOnly
}

/// <summary>
/// Objet interactif sur lequel on peut cliquer pour déplacer le joueur à proximité.
/// Utilise un système de compteur pour gérer plusieurs sources de blocage.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [Header("Type d'interaction")]
    [Tooltip("MoveToObject: Le joueur se déplace vers l'objet. ClickOnly: Interaction immédiate au clic.")]
    [SerializeField] protected InteractionType interactionType = InteractionType.MoveToObject;

    [Header("Paramètres d'interaction")]
    [Tooltip("Distance à laquelle le joueur s'arrêtera de l'objet")]
    [SerializeField] protected float interactionDistance = 1f;

    [Tooltip("Afficher un outline ou un effet visuel au survol")]
    [SerializeField] private bool highlightOnHover = true;

    private Outline outlineComponent;
    private bool isHovered = false;

    // Système de compteur pour gérer l'interactivité
    private int canInteract = 0;

    // Pour MoveToObject: suivi du déplacement du joueur
    private PlayerControl.PlayerController player;
    private bool isPlayerMovingToThis = false;

    private void OnEnable()
    {
        // S'enregistrer quand l'objet est activé
        GameManager.RegisterInteractable(this);
    }

    private void OnDisable()
    {
        // Se désenregistrer quand l'objet est désactivé
        GameManager.UnregisterInteractable(this);

        if (isHovered && outlineComponent != null)
        {
            outlineComponent.enabled = false;
            isHovered = false;
        }
    }

    protected virtual void Start()
    {
        // Récupérer le composant Outline s'il existe
        outlineComponent = GetComponent<Outline>();

        // Désactiver l'outline au départ si présent
        if (outlineComponent != null && highlightOnHover)
        {
            outlineComponent.enabled = false;
        }

        // Vérifier qu'il y a un collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"InteractableObject sur {gameObject.name}: Aucun Collider trouvé!");
        }

        // Trouver le joueur (nécessaire pour MoveToObject)
        player = Object.FindFirstObjectByType<PlayerControl.PlayerController>();
    }

    protected virtual void Update()
    {
        // Vérifier si le joueur est arrivé à destination (pour MoveToObject)
        CheckPlayerArrival();

        // Vérifier si l'objet peut être interagi AVANT tout le reste
        if (!CanInteract())
        {
            // Désactiver le hover si l'objet n'est plus interactif
            if (isHovered)
            {
                OnHoverExit();
            }
            return;
        }

        if (Mouse.current == null)
        {
            Debug.LogWarning($"InteractableObject ({gameObject.name}): Mouse.current est null!");
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning($"InteractableObject ({gameObject.name}): Camera.main est null!");
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Vérifier si la souris survole cet objet
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // Souris sur cet objet
                if (!isHovered)
                {
                    OnHoverEnter();
                }

                // Détection du clic
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    OnClicked();
                }
            }
            else
            {
                // Souris sur autre chose
                if (isHovered)
                {
                    OnHoverExit();
                }
            }
        }
        else
        {
            // Aucun objet sous la souris
            if (isHovered)
            {
                OnHoverExit();
            }
        }
    }

    /// <summary>
    /// Vérifie si le joueur est arrivé à destination (pour MoveToObject).
    /// </summary>
    private void CheckPlayerArrival()
    {
        if (interactionType != InteractionType.MoveToObject || !isPlayerMovingToThis || player == null)
        {
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool isMoving = player.IsMoving();
        float triggerDistance = interactionDistance + 0.5f;

        if (distance <= triggerDistance && !isMoving)
        {
            isPlayerMovingToThis = false;
            OnInteracted();
        }
    }

    private void OnHoverEnter()
    {
        isHovered = true;

        // Activer l'outline si disponible
        if (outlineComponent != null && highlightOnHover)
        {
            outlineComponent.enabled = true;
        }

    }

    private void OnHoverExit()
    {
        isHovered = false;

        // Désactiver l'outline
        if (outlineComponent != null && highlightOnHover)
        {
            outlineComponent.enabled = false;
        }
    }

    protected virtual void OnClicked()
    {
        // Double vérification avant d'accepter le clic
        if (!CanInteract())
        {
            return;
        }

        // Comportement selon le type d'interaction
        switch (interactionType)
        {
            case InteractionType.MoveToObject:
                HandleMoveToObjectInteraction();
                break;

            case InteractionType.ClickOnly:
                HandleClickOnlyInteraction();
                break;
        }
    }

    /// <summary>
    /// Gère l'interaction avec déplacement vers l'objet.
    /// </summary>
    private void HandleMoveToObjectInteraction()
    {
        if (player == null)
        {
            Debug.LogWarning($"{gameObject.name}: PlayerController non trouvé!");
            return;
        }

        // Vérifier si le joueur est déjà à proximité
        float currentDistance = Vector3.Distance(player.transform.position, transform.position);
        float triggerDistance = interactionDistance + 0.5f;

        if (currentDistance <= triggerDistance)
        {
            // Déjà assez proche, interagir immédiatement
            OnInteracted();
        }
        else
        {
            // Calculer la position cible 
            Vector3 targetPosition = CalculateTargetPosition(player.transform.position);

            // Demander au joueur de se déplacer vers cette position
            player.MoveToPosition(targetPosition);

            // Marquer qu'on attend l'arrivée du joueur
            isPlayerMovingToThis = true;
        }
    }

    /// <summary>
    /// Gère l'interaction directe sans déplacement.
    /// </summary>
    private void HandleClickOnlyInteraction()
    {
        OnInteracted();
    }

    /// <summary>
    /// Appelé lors d'une interaction. Peut être override par les classes enfants.
    /// Pour MoveToObject: appeler cette méthode quand le joueur arrive à destination.
    /// Pour ClickOnly: appelé automatiquement au clic.
    /// </summary>
    protected virtual void OnInteracted()
    {
        // Rien par défaut - les classes enfants peuvent override cette méthode
        Debug.Log($"{gameObject.name} a été interagi");
    }

    /// <summary>
    /// Calcule la position cible à interactionDistance de l'objet.
    /// </summary>
    private Vector3 CalculateTargetPosition(Vector3 playerPosition)
    {
        // Direction du joueur vers l'objet
        Vector3 direction = (transform.position - playerPosition).normalized;

        // Position cible = position de l'objet - direction * distance
        Vector3 targetPos = transform.position - (direction * interactionDistance);

        // Garder la hauteur du joueur (Y)
        targetPos.y = playerPosition.y;

        return targetPos;
    }

    // Gizmo pour visualiser la distance d'interaction
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    #region Système d'interactivité (compteur)

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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false; // UI bloque l'interaction
            }
        }

        return true;
    }


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


    public void ResetCounter()
    {
        canInteract = 0;
    }


    public int GetCounter()
    {
        return canInteract;
    }

    #endregion
}
