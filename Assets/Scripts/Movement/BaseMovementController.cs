using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerControl
{
    /// <summary>
    /// Classe de base pour tout contrôleur de mouvement (joueur ou bot).
    /// Contient la logique commune de déplacement, animation et son.
    /// </summary>
    public abstract class BaseMovementController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float rotationSpeed = 10f; // Vitesse de rotation en degrés par seconde

        [Header("Animation")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected float animationDampTime = 0.1f;
        [SerializeField] protected string runParameterName = "Run";

        [Header("Audio")]
        [SerializeField] protected AudioSource footstepsAudioSource;
        [SerializeField] protected AudioClip footstepsSound;
        [SerializeField][Range(0f, 1f)] protected float footstepsVolume = 0.5f;

        protected NavMeshPath path;
        protected int currentPathIndex;
        protected bool isMoving;
        protected List<Vector3> destinationQueue = new List<Vector3>();

        protected virtual void Awake()
        {
            path = new NavMeshPath();

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (footstepsAudioSource == null)
            {
                footstepsAudioSource = GetComponent<AudioSource>();
            }
        }

        protected virtual void Update()
        {
            if (isMoving)
            {
                UpdateRunAnimation(1.0f);
                PlayFootstepsSound();
                MoveAlongPath();
            }
            else
            {
                UpdateRunAnimation(0.0f);
                StopFootstepsSound();
            }
        }

        /// <summary>
        /// Met à jour l'animation de course
        /// </summary>
        protected virtual void UpdateRunAnimation(float targetValue)
        {
            if (animator != null && !string.IsNullOrEmpty(runParameterName))
            {
                float currentValue = animator.GetFloat(runParameterName);
                float newValue = Mathf.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-animationDampTime * Time.deltaTime * 40));
                animator.SetFloat(runParameterName, newValue);
            }
        }

        /// <summary>
        /// Déplace l'entité le long du chemin calculé
        /// </summary>
        protected virtual void MoveAlongPath()
        {
            if (currentPathIndex < path.corners.Length)
            {
                Vector3 targetPosition = path.corners[currentPathIndex];
                Vector3 direction = (targetPosition - transform.position).normalized;

                // Faire pivoter dans la direction du mouvement
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * 100f * Time.deltaTime
                    );
                }

                transform.position += direction * moveSpeed * Time.deltaTime;

                // Si on atteint la position cible
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

        /// <summary>
        /// Calcule et démarre le déplacement vers la prochaine destination
        /// </summary>
        protected virtual void MoveToNextDestination()
        {
            if (destinationQueue.Count > 0)
            {
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
        /// Déplace l'entité vers une position spécifique
        /// </summary>
        public virtual void MoveToPosition(Vector3 targetPosition)
        {
            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(targetPosition, out navHit, 5f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"{gameObject.name}: La position cible n'est pas sur le NavMesh!");
                return;
            }

            destinationQueue.Clear();
            destinationQueue.Add(navHit.position);

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
        /// Vérifie si l'entité est en train de se déplacer
        /// </summary>
        public virtual bool IsMoving()
        {
            return isMoving;
        }

        /// <summary>
        /// Arrête le mouvement en cours
        /// </summary>
        public virtual void StopMovement()
        {
            isMoving = false;
            destinationQueue.Clear();
            StopFootstepsSound();
        }

        #region Audio

        /// <summary>
        /// Joue le son de pas en boucle si ce n'est pas déjà en cours
        /// </summary>
        protected virtual void PlayFootstepsSound()
        {
            if (footstepsAudioSource == null || footstepsSound == null)
                return;

            if (!footstepsAudioSource.isPlaying)
            {
                footstepsAudioSource.clip = footstepsSound;
                footstepsAudioSource.volume = footstepsVolume;
                footstepsAudioSource.loop = true;
                footstepsAudioSource.Play();
            }
        }

        /// <summary>
        /// Arrête le son de pas
        /// </summary>
        protected virtual void StopFootstepsSound()
        {
            if (footstepsAudioSource != null && footstepsAudioSource.isPlaying)
            {
                footstepsAudioSource.Stop();
            }
        }

        #endregion

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            // Dessiner le chemin actuel
            if (path != null && path.corners.Length > 0)
            {
                Gizmos.color = Color.blue;

                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                }

                Gizmos.color = Color.green;
                foreach (Vector3 corner in path.corners)
                {
                    Gizmos.DrawSphere(corner, 0.2f);
                }

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

                for (int i = 0; i < destinationQueue.Count; i++)
                {
                    Gizmos.DrawWireSphere(destinationQueue[i], 0.5f);

                    if (i > 0)
                    {
                        Gizmos.DrawLine(destinationQueue[i - 1], destinationQueue[i]);
                    }
                    else if (isMoving)
                    {
                        Gizmos.DrawLine(transform.position, destinationQueue[i]);
                    }
                }

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
