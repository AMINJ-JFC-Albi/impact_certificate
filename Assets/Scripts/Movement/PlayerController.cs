using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace PlayerControl
{
    public class PlayerController : BaseMovementController
    {
        [Header("Player Specific")]
        [SerializeField] private Camera mainCamera;

        [Header("Continuous Path Creation")]
        [SerializeField] private float continuousPathDelay = 0.1f;
        private float lastPathCreationTime = 0f;
        private bool isRightClickHeld = false;

        [Header("Health System")]
        private PlayerHealth playerHealth;

        protected override void Awake()
        {
            base.Awake();

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            playerHealth = GetComponent<PlayerHealth>();

        }

        private void Start()
        {
            mainCamera = Camera.main;

            if (InputManager.Instance == null)
            {
                Debug.LogError("InputManager.Instance est null! Assurez-vous qu'un GameObject avec InputManager existe dans la scène.");
                return;
            }

            InputManager.Instance.RightClickMovement.started += OnRightClickStarted;
            InputManager.Instance.RightClickMovement.canceled += OnRightClickCanceled;
        }

        private void OnRightClickStarted(InputAction.CallbackContext ctx)
        {
            isRightClickHeld = true;
        }

        private void OnRightClickCanceled(InputAction.CallbackContext ctx)
        {
            isRightClickHeld = false;
        }


        protected override void Update()
        {
            if (InputManager.Instance == null)
                return;

            // Ensure actions are initialized
            if (InputManager.Instance.RightClickMovement == null)
                return;

            if (InputManager.Instance.RightClickMovement.WasPerformedThisFrame())
            {
                HandleMovement();
                lastPathCreationTime = Time.time;
            }

            if (isRightClickHeld && Time.time > lastPathCreationTime + continuousPathDelay)
            {
                HandleMovement();
                lastPathCreationTime = Time.time;
            }

            // Laisser la classe de base gérer le mouvement, l'animation et le son
            base.Update();
        }

        private void HandleMovement()
        {
            // Bloquer la création de nouveaux mouvements si les interactions sont désactivées
            if (!GameManager.InteractionsEnabled)
            {
                return;
            }

            if (mainCamera == null || Mouse.current == null || InputManager.Instance == null)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0f));
            bool isShiftPressed = InputManager.Instance.ShiftAction.IsPressed();

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPoint = hit.point;
                NavMeshHit navHit;

                // Si le point ciblé n'est pas sur la NavMesh
                if (!NavMesh.SamplePosition(targetPoint, out navHit, 100f, NavMesh.AllAreas))
                {
                    return;
                }

                Vector3 validDestination = navHit.position;

                // Si shift est pressé, ajouter à la liste de destinations
                if (isShiftPressed)
                {
                    destinationQueue.Add(validDestination);

                    if (!isMoving)
                    {
                        MoveToNextDestination();
                    }
                }
                else
                {
                    // Sans shift, effacer toutes les destinations précédentes
                    destinationQueue.Clear();
                    destinationQueue.Add(validDestination);

                    // Calculer le chemin vers cette destination
                    if (NavMesh.CalculatePath(transform.position, validDestination, NavMesh.AllAreas, path))
                    {
                        if (path.corners.Length > 1)
                        {
                            currentPathIndex = 1;
                            isMoving = true;
                        }
                    }
                }
            }
        }

        void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.RightClickMovement.started -= OnRightClickStarted;
                InputManager.Instance.RightClickMovement.canceled -= OnRightClickCanceled;
            }
        }


#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            // Dessiner le chemin actuel
            if (path != null && path.corners.Length > 0)
            {
                Gizmos.color = Color.blue;

                // Dessiner une ligne entre chaque point du chemin
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                }

                // Dessiner des sphères aux points d'intersection
                Gizmos.color = Color.green;
                foreach (Vector3 corner in path.corners)
                {
                    Gizmos.DrawSphere(corner, 0.2f);
                }

                // Marquer le point actuel sur le chemin
                if (isMoving && currentPathIndex < path.corners.Length)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(path.corners[currentPathIndex], 0.3f);
                }
            }

            // Dessiner les points de destination en file d'attente
            if (destinationQueue.Count > 0)
            {
                Gizmos.color = Color.magenta;

                // Dessiner toutes les destinations dans la file d'attente
                for (int i = 0; i < destinationQueue.Count; i++)
                {
                    Gizmos.DrawWireSphere(destinationQueue[i], 0.5f);

                    // Dessiner des lignes entre les points pour montrer l'ordre
                    if (i > 0)
                    {
                        Gizmos.DrawLine(destinationQueue[i - 1], destinationQueue[i]);
                    }
                    else if (isMoving)
                    {
                        // Ligne depuis la position actuelle vers la première destination
                        Gizmos.DrawLine(transform.position, destinationQueue[i]);
                    }
                }

                // Mettre en évidence la prochaine destination avec une couleur différente
                if (destinationQueue.Count > 0)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(destinationQueue[0], 0.6f);
                    Gizmos.DrawSphere(destinationQueue[0], 0.2f);
                }
            }
        }
#endif
    }
}

