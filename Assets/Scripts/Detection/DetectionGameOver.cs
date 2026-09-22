using UnityEngine;
using Detection;

/// <summary>
/// Composant qui déclenche un Game Over quand le joueur est détecté.
/// </summary>
[RequireComponent(typeof(BaseDetection))]
public class DetectionGameOver : MonoBehaviour
{
    private BaseDetection detection;

    private void Awake()
    {
        detection = GetComponent<BaseDetection>();
    }

    private void OnEnable()
    {
        if (detection != null)
        {
            detection.OnTargetDetected += OnPlayerDetected;
        }
    }

    private void OnDisable()
    {
        if (detection != null)
        {
            detection.OnTargetDetected -= OnPlayerDetected;
        }
    }

    private void OnPlayerDetected(Transform player)
    {
        Debug.LogWarning($"GAME OVER ! {gameObject.name} a attrapé le joueur !");

        // TODO: Appeler votre système de Game Over ici

    }
}
