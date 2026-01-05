using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;
using UnityEngine.Events;

/// <summary>
/// Système de patrouille pour les bots NPC.
/// Suit une liste de points de passage avec délais et retour en arrière.
/// Hérite de BaseMovementController pour réutiliser les systèmes d'animation et de son.
/// </summary>
public class BotPatrol : BaseMovementController
{
    [System.Serializable]
    public class PatrolPoint
    {
        [Tooltip("Position du point de patrouille")]
        public Transform waypoint;

        [Tooltip("Temps d'attente à ce point (en secondes)")]
        public float waitTime = 2f;
    }

    [Header("Paramètres de patrouille")]
    [SerializeField] private List<PatrolPoint> patrolPoints = new List<PatrolPoint>();

    [Tooltip("Temps d'attente à la position de départ (en secondes)")]
    [SerializeField] private float startPositionWaitTime = 2f;
    
    [Tooltip("Stop la patrouille aux dernier point")]
    [SerializeField] private bool stopAtEnd = false;

    [Tooltip("Si true, fait des allers-retours. Si false, boucle du dernier au premier point")]
    [SerializeField] private bool reversePatrol = true;

    [Tooltip("Démarre automatiquement la patrouille au Start()")]
    [SerializeField] private bool autoStart = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color pathColor = Color.cyan;

    [Header("Evenement de patrouille")]
    public UnityEvent<int, int> pathEvent;

    private int currentPointIndex = 0;
    private bool isMovingForward = true;
    private bool isPatrolling = false;
    private bool isWaiting = false;
    private Vector3 startPosition;

    protected override void Awake()
    {
        base.Awake();
        // Sauvegarder la position de départ
        startPosition = transform.position;
    }

    private void Start()
    {
        if (autoStart && patrolPoints.Count > 0)
        {
            StartPatrol();
        }
    }

    protected override void Update()
    {
        base.Update();

        // Vérifier si le bot a atteint sa destination
        if (!isWaiting && !isMoving)
        {
            pathEvent.Invoke(currentPointIndex, patrolPoints.Count-1);
            StartCoroutine(WaitAtPoint());
        }
    }

    /// <summary>
    /// Assigne une patrouille
    /// </summary>
    public void SetPatrolPoints(List<PatrolPoint> newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }

    /// <summary>
    /// Démarre la patrouille
    /// </summary>
    public void StartPatrol()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning($"BotPatrol '{gameObject.name}': Aucun point de patrouille défini!");
            return;
        }

        isPatrolling = true;
        currentPointIndex = 0;
        isMovingForward = true;
        GoToCurrentPoint();
        Debug.LogError("where !" + gameObject.name);
    }

    /// <summary>
    /// Arrête la patrouille
    /// </summary>
    public void StopPatrol()
    {
        isPatrolling = false;
        StopMovement();
    }

    /// <summary>
    /// Met en pause la patrouille
    /// </summary>
    public void PausePatrol()
    {
        isPatrolling = false;
    }

    /// <summary>
    /// Reprend la patrouille après une pause
    /// </summary>
    public void ResumePatrol()
    {
        if (patrolPoints.Count > 0)
        {
            isPatrolling = true;
        }
    }

    /// <summary>
    /// Déplace le bot vers le point de patrouille actuel

    /// </summary>
    private void GoToCurrentPoint()
    {
        if (!isPatrolling) return;
        if (currentPointIndex == -1)
        {
            MoveToPosition(startPosition);
            return;
        }

        if (currentPointIndex < 0 || currentPointIndex >= patrolPoints.Count)
        {
            Debug.LogError($"BotPatrol '{gameObject.name}': Index de point invalide ({currentPointIndex})");
            return;
        }

        PatrolPoint point = patrolPoints[currentPointIndex];
        if (point.waypoint != null)
        {
            MoveToPosition(point.waypoint.position);
        }
        else
        {
            Debug.LogWarning($"BotPatrol '{gameObject.name}': Waypoint {currentPointIndex} est null!");
        }
    }

    /// <summary>
    /// Attend au point actuel puis passe au suivant
    /// </summary>
    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        // Attendre le délai configuré
        float waitTime = 0f;
        if (currentPointIndex == -1)
        {
            // Position de départ : utiliser le délai configuré
            waitTime = startPositionWaitTime;
        }
        else if (currentPointIndex >= 0 && currentPointIndex < patrolPoints.Count)
        {
            waitTime = patrolPoints[currentPointIndex].waitTime;
        }

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // Passer au point suivant
        MoveToNextPoint();

        isWaiting = false;
    }

    /// <summary>
    /// Calcule le prochain point de patrouille
    /// </summary>
    private void MoveToNextPoint()
    {
        if (patrolPoints.Count == 0) return;

        // Si on a atteint le dernier point, et que l'on veut s'arrêter, on stop la patrouille
        if (currentPointIndex >= patrolPoints.Count-1 && stopAtEnd)
        {
            StopPatrol();
            return;
        }

        // Gérer le cas où il n'y a qu'un seul point
        if (patrolPoints.Count == 1)
        {
            currentPointIndex = 0;
            GoToCurrentPoint();
            return;
        }

        if (reversePatrol)
        {
            // Mode aller-retour : Position départ → Waypoints → Position départ
            if (isMovingForward)
            {
                currentPointIndex++;

                // Si on atteint la fin de la liste, inverser la direction
                if (currentPointIndex >= patrolPoints.Count)
                {
                    currentPointIndex = patrolPoints.Count - 1;
                    isMovingForward = false;
                }
            }
            else
            {
                currentPointIndex--;

                if (currentPointIndex < -1)
                {
                    currentPointIndex = -1;
                    isMovingForward = true;
                }
            }
        }
        else
        {
            currentPointIndex++;
            if (currentPointIndex >= patrolPoints.Count)
            {
                currentPointIndex = -1;
            }
        }
        GoToCurrentPoint();
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        // Appeler les gizmos de la classe de base
        base.OnDrawGizmosSelected();

        if (!showDebugGizmos) return;

        Gizmos.color = pathColor;

        // Dessiner la position de départ (sphère verte)
        Gizmos.color = Color.green;
        Vector3 startPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(startPos, 0.5f);
        Gizmos.DrawSphere(startPos, 0.2f);

        if (patrolPoints.Count == 0) return;

        // Ligne depuis la position de départ vers le premier waypoint
        if (patrolPoints[0].waypoint != null)
        {
            Gizmos.color = pathColor;
            Gizmos.DrawLine(startPos, patrolPoints[0].waypoint.position);
        }

        // Dessiner les lignes entre les waypoints
        for (int i = 0; i < patrolPoints.Count - 1; i++)
        {
            if (patrolPoints[i].waypoint != null && patrolPoints[i + 1].waypoint != null)
            {
                Gizmos.DrawLine(patrolPoints[i].waypoint.position, patrolPoints[i + 1].waypoint.position);
            }
        }

        // Mode boucle : relier le dernier waypoint à la position de départ
        if (!reversePatrol && patrolPoints.Count > 0)
        {
            if (patrolPoints[patrolPoints.Count - 1].waypoint != null)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Count - 1].waypoint.position, startPos);
            }
        }
        // Mode aller-retour : relier le dernier waypoint à la position de départ
        else if (reversePatrol && patrolPoints.Count > 0)
        {
            if (patrolPoints[patrolPoints.Count - 1].waypoint != null)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Count - 1].waypoint.position, startPos);
            }
        }

        // Dessiner les points de patrouille
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            if (patrolPoints[i].waypoint != null)
            {
                // Point actuel en rouge, autres en jaune
                Gizmos.color = (Application.isPlaying && i == currentPointIndex) ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(patrolPoints[i].waypoint.position, 0.5f);

                // Afficher le numéro du point (numérotation depuis 1 car 0 = position départ)
                UnityEditor.Handles.Label(
                    patrolPoints[i].waypoint.position + Vector3.up * 1f,
                    $"Point {i + 1}\nWait: {patrolPoints[i].waitTime}s"
                );
            }
        }

        // Marquer la position de départ en rouge si c'est le point actuel
        if (Application.isPlaying && currentPointIndex == -1)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPos, 0.6f);
        }

        // Dessiner la direction
        if (Application.isPlaying && patrolPoints.Count > 0)
        {
            Gizmos.color = isMovingForward ? Color.green : Color.blue;
            Vector3 directionArrow = transform.position + Vector3.up * 2f;
            Vector3 direction = isMovingForward ? transform.forward : -transform.forward;
            Gizmos.DrawRay(directionArrow, direction * 2f);
        }
    }
#endif

}
