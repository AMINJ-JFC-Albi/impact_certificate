using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Active ou désactive un NavMeshObstacle.
/// Utile pour les portes, passages, etc.
/// </summary>
public class ToggleNavMeshObstacle : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle navMeshObstacle;

    /// <summary>
    /// Active l'obstacle (bloque le passage).
    /// </summary>
    public void EnableObstacle()
    {
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = true;
        }
    }

    /// <summary>
    /// Désactive l'obstacle (permet le passage).
    /// </summary>
    public void DisableObstacle()
    {
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = false;
        }
    }

    /// <summary>
    /// Inverse l'état de l'obstacle.
    /// </summary>
    public void ToggleObstacle()
    {
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = !navMeshObstacle.enabled;
        }
    }

    /// <summary>
    /// Définit l'état de l'obstacle.
    /// </summary>
    /// <param name="enabled">True pour activer, False pour désactiver</param>
    public void SetObstacle(bool enabled)
    {
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = enabled;
        }
    }
}
