using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Objet interactif sur lequel on peut cliquer pour déplacer le joueur à proximité.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [Header("Paramètres d'interaction")]
    [Tooltip("Distance à laquelle le joueur s'arrêtera de l'objet")]
    [SerializeField] protected float interactionDistance = 1f;

    [Tooltip("Afficher un outline ou un effet visuel au survol")]
    [SerializeField] private bool highlightOnHover = true;

    private Outline outlineComponent;
    private bool isHovered = false;

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
    }

    protected virtual void Update()
    {
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

        // Vérifier si la souris est au-dessus d'un élément UI
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // La souris est sur l'UI, ne pas traiter les interactions avec les objets 3D
            if (isHovered)
            {
                OnHoverExit();
            }
            return;
        }

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
        // Trouver le joueur
        PlayerControl.PlayerController player = FindObjectOfType<PlayerControl.PlayerController>();

        if (player == null)
        {
            Debug.LogError("InteractableObject: Aucun PlayerController trouvé dans la scène!");
            return;
        }

        // Calculer la position cible 
        Vector3 targetPosition = CalculateTargetPosition(player.transform.position);

        // Demander au joueur de se déplacer vers cette position
        player.MoveToPosition(targetPosition);

        // Appeler l'action après le déplacement
        OnPlayerReachedDestination(player);
    }

    /// <summary>
    /// Appelé quand le joueur atteint la destination. Peut être override par les classes enfants.
    /// </summary>
    protected virtual void OnPlayerReachedDestination(PlayerControl.PlayerController player)
    {
        // Rien par défaut - les classes enfants peuvent override cette méthode
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

    private void OnDisable()
    {
        // S'assurer de désactiver l'outline
        if (isHovered && outlineComponent != null)
        {
            outlineComponent.enabled = false;
            isHovered = false;
        }
    }

    // Gizmo pour visualiser la distance d'interaction
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
