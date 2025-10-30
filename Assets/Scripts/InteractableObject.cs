using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objet interactif sur lequel on peut cliquer pour déplacer le joueur à proximité.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [Header("Paramètres d'interaction")]
    [Tooltip("Distance à laquelle le joueur s'arrêtera de l'objet")]
    [SerializeField] private float interactionDistance = 1f;

    [Tooltip("Afficher un outline ou un effet visuel au survol")]
    [SerializeField] private bool highlightOnHover = true;

    private Outline outlineComponent;
    private bool isHovered = false;

    private void Start()
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

    private void Update()
    {
        if (Mouse.current == null) return;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        // Vérifier si la souris survole cet objet
        Vector2 mousePos = Mouse.current.position.ReadValue();
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

    private void OnClicked()
    {
        Debug.Log($"Clic sur l'objet interactif: {gameObject.name}");

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
