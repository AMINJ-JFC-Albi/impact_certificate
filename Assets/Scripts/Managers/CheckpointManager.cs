using UnityEngine;

/// <summary>
/// Gère le point de respawn du joueur
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("État actuel")]
    [SerializeField] private Transform currentSpawnPoint;
    [SerializeField] private Vector3 currentSpawnPosition;
    [SerializeField] private Quaternion currentSpawnRotation;

    private GameObject playerObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Trouver le joueur
        playerObject = GameObject.FindGameObjectWithTag("Player");

        // Enregistrer la position de départ du joueur comme spawn initial
        if (playerObject != null)
        {
            currentSpawnPosition = playerObject.transform.position;
            currentSpawnRotation = playerObject.transform.rotation;
        }
    }

    /// <summary>
    /// Définit un nouveau point de spawn (appelé par les InteractableObjects)
    /// </summary>
    public void SetSpawnPoint(Transform spawnPoint)
    {
        if (spawnPoint != null)
        {
            currentSpawnPoint = spawnPoint;
            currentSpawnPosition = spawnPoint.position;
            currentSpawnRotation = spawnPoint.rotation;
        }
    }

    /// <summary>
    /// Définit un nouveau point de spawn par position/rotation
    /// </summary>
    public void SetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        currentSpawnPosition = position;
        currentSpawnRotation = rotation;
        currentSpawnPoint = null;
    }

    /// <summary>
    /// Respawn le joueur au dernier checkpoint enregistré (appelé à la mort)
    /// </summary>
    public void RespawnPlayer()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerObject != null)
        {
            // Téléporter le joueur
            playerObject.transform.position = currentSpawnPosition;
            playerObject.transform.rotation = currentSpawnRotation;

            // Réinitialiser la santé
            PlayerControl.PlayerHealth playerHealth = playerObject.GetComponent<PlayerControl.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ResetHealth();
            }
        }
        else
        {
            Debug.LogError("Impossible de respawn le joueur!");
        }
    }

    /// <summary>
    /// Retourne la position de spawn actuelle
    /// </summary>
    public Vector3 GetCurrentSpawnPosition()
    {
        return currentSpawnPosition;
    }
}
