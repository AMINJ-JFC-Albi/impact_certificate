using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;



namespace PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;


        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Continuous Path Creation")]
        [SerializeField] private float continuousPathDelay = 0.1f;
        private float lastPathCreationTime = 0f;
        private bool isRightClickHeld = false;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private float animationDampTime = 0.1f;


        private NavMeshPath path;
        private int currentPathIndex;
        private bool isMoving;

        private List<Vector3> destinationQueue = new List<Vector3>();


        void Awake()
        {
            path = new NavMeshPath();

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
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


        void Update()
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

            if (isMoving)
            {
                UpdateRunAnimation(1.0f);
                MoveAlongPath();
            }

            else
            {
                UpdateRunAnimation(0.0f);
            }
        }

        private void UpdateRunAnimation(float targetValue)
        {
            if (animator != null)
            {

                float currentValue = animator.GetFloat("Run");
                float newValue = Mathf.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-animationDampTime * Time.deltaTime * 40));
                animator.SetFloat("Run", newValue);
            }
        }

        private void HandleMovement()
        {
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

        private void MoveAlongPath()
        {
            if (currentPathIndex < path.corners.Length)
            {
                Vector3 targetPosition = path.corners[currentPathIndex];

                Vector3 direction = (targetPosition - transform.position).normalized;

                // Faire pivoter le personnage dans la direction du mouvement
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                }

                transform.position += direction * moveSpeed * Time.deltaTime;

                // si on atteint la position cible
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    currentPathIndex++;
                }
            }
            else
            {
                // Si on a atteint la destination, passer à la suivante s'il y en a
                if (destinationQueue.Count > 0)
                {
                    destinationQueue.RemoveAt(0);

                    if (destinationQueue.Count > 0)
                    {
                        MoveToNextDestination();
                        return;
                    }
                }

                isMoving = false;
            }
        }

        private void MoveToNextDestination()
        {
            if (destinationQueue.Count > 0)
            {
                // Calculer le chemin vers la prochaine destination
                if (NavMesh.CalculatePath(transform.position, destinationQueue[0], NavMesh.AllAreas, path))
                {
                    if (path.corners.Length > 1)
                    {
                        currentPathIndex = 1;
                        isMoving = true;
                    }
                }
            }
        }

        /// <summary>
        /// Déplace le joueur vers une position spécifique (utilisé par les objets interactifs).
        /// </summary>
        public void MoveToPosition(Vector3 targetPosition)
        {
            // Vérifier que la position est sur le NavMesh
            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(targetPosition, out navHit, 5f, NavMesh.AllAreas))
            {
                Debug.LogWarning("PlayerController: La position cible n'est pas sur le NavMesh!");
                return;
            }

            // Effacer la file d'attente et ajouter la nouvelle destination
            destinationQueue.Clear();
            destinationQueue.Add(navHit.position);

            // Calculer le chemin
            if (NavMesh.CalculatePath(transform.position, navHit.position, NavMesh.AllAreas, path))
            {
                if (path.corners.Length > 1)
                {
                    currentPathIndex = 1;
                    isMoving = true;
                }
            }
        }

        /// <summary>
        /// Vérifie si le joueur est en train de se déplacer.
        /// </summary>
        public bool IsMoving()
        {
            return isMoving;
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
        private void OnDrawGizmos()
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

