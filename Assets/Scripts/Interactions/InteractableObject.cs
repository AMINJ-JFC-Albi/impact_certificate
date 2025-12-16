using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Type d'interaction pour l'objet interactif
/// </summary>
public enum InteractionType
{
    MoveToObject,  // Le joueur se déplace vers l'objet avant d'interagir
    ClickOnly      // Interaction immédiate au clic
}

/// <summary>
/// Objet interactif cliquable.
/// Gère le déplacement du joueur, l'outline et l'interaction via un système de compteur.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [Header("Type d'interaction")]
    [SerializeField] protected InteractionType interactionType = InteractionType.MoveToObject;

    [Header("Paramètres d'interaction")]
    [SerializeField] protected float interactionDistance = 1f;

    private Outline outlineComponent;
    private bool isHovered = false;
    private int canInteract = 0; // Système de compteur pour gérer plusieurs sources de blocage
    private PlayerControl.PlayerController player;
    private bool isPlayerMovingToThis = false;

    private void OnEnable()
    {
        GameManager.RegisterInteractable(this);
    }

    private void OnDisable()
    {
        GameManager.UnregisterInteractable(this);
    }

    protected virtual void Start()
    {
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
        {
            outlineComponent = GetComponentInChildren<Outline>();
        }

        player = Object.FindFirstObjectByType<PlayerControl.PlayerController>();
    }
    protected virtual void Update()
    {
        CheckPlayerArrival();

        // Vérifier si l'objet peut être interagi
        if (!CanInteract())
        {
            if (isHovered)
            {
                OnHoverExit();
            }
            return;
        }

        if (Mouse.current == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isHovered)
                {
                    OnHoverEnter();
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    OnClicked();
                }
            }
            else if (isHovered)
            {
                OnHoverExit();
            }
        }
        else if (isHovered)
        {
            OnHoverExit();
        }
    }

    /// <summary>
    /// Vérifie si le joueur est arrivé à destination (pour MoveToObject)
    /// </summary>
    private void CheckPlayerArrival()
    {
        if (interactionType != InteractionType.MoveToObject || !isPlayerMovingToThis || player == null)
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        float triggerDistance = interactionDistance + 0.5f;

        if (distance <= triggerDistance && !player.IsMoving())
        {
            isPlayerMovingToThis = false;
            OnInteracted();
        }
    }

    private void OnHoverEnter()
    {
        isHovered = true;
    }

    private void OnHoverExit()
    {
        isHovered = false;
    }

    protected virtual void OnClicked()
    {
        if (!CanInteract()) return;

        switch (interactionType)
        {
            case InteractionType.MoveToObject:
                HandleMoveToObjectInteraction();
                break;
            case InteractionType.ClickOnly:
                OnInteracted();
                break;
        }
    }

    private void HandleMoveToObjectInteraction()
    {
        if (player == null) return;

        float currentDistance = Vector3.Distance(player.transform.position, transform.position);
        float triggerDistance = interactionDistance + 0.5f;

        if (currentDistance <= triggerDistance)
        {
            // Déjà à proximité, interagir immédiatement
            OnInteracted();
        }
        else
        {
            // Déplacer le joueur vers l'objet
            Vector3 targetPosition = CalculateTargetPosition(player.transform.position);
            player.MoveToPosition(targetPosition);
            isPlayerMovingToThis = true;
        }
    }

    /// <summary>
    /// Appelé lors d'une interaction. Override dans les classes enfants pour définir le comportement.
    /// </summary>
    protected virtual void OnInteracted()
    {
    }

    /// <summary>
    /// Calcule la position cible à interactionDistance de l'objet
    /// </summary>
    private Vector3 CalculateTargetPosition(Vector3 playerPosition)
    {
        Vector3 direction = (transform.position - playerPosition).normalized;
        Vector3 targetPos = transform.position - (direction * interactionDistance);
        targetPos.y = playerPosition.y;
        return targetPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    #region Système d'interactivité

    /// <summary>
    /// Vérifie si l'objet peut être interagi (compteur à 0 et pas d'UI au-dessus)
    /// </summary>
    public bool CanInteract()
    {
        if (canInteract > 0) return false;

        if (Mouse.current != null && EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return false;
        }

        return true;
    }

    /// <summary>
    /// Active/désactive l'interactivité via un système de compteur
    /// </summary>
    public void SetActive(bool active)
    {
        if (active)
        {
            canInteract--;
            if (canInteract < 0) canInteract = 0;
        }
        else
        {
            canInteract++;
        }

        UpdateOutlineState();
    }

    /// <summary>
    /// Met à jour l'état de l'outline selon si l'objet peut être interagi
    /// </summary>
    public void UpdateOutlineState()
    {
        if (outlineComponent == null) return;

        // L'outline est activé si le compteur est à 0 (pas de blocage)
        outlineComponent.enabled = (canInteract == 0);
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
