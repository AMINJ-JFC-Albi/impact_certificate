using UnityEngine;
using System.Collections;

/// <summary>
/// Déclenche des actions quand on clique sur l'objet.
/// Peut activer/désactiver des objets directement ou appeler ActionManager pour des actions complexes.
/// </summary>
public class ActionTrigger : InteractableObject
{
    [Header("Objets à activer/désactiver")]
    [Tooltip("Objets à activer lors de l'interaction")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Tooltip("Délai avant d'activer les objets (0 = immédiat)")]
    [SerializeField] private float delayBeforeActivate = 0f;

    [Tooltip("Objets à désactiver lors de l'interaction ")]
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Tooltip("Délai avant de désactiver les objets (0 = immédiat)")]
    [SerializeField] private float delayBeforeDeactivate = 0f;

    [Header("Action complexe (optionnel)")]
    [Tooltip("Nom de l'action complexe à exécuter dans l'ActionManager (laisser vide si pas nécessaire)")]
    [SerializeField] private string actionName = "";

    [Tooltip("Objectif pour cette action (remplace l'objectif par défaut de l'action si rempli)")]
    [SerializeField] private string objective = "";

    [Header("Checkpoint")]
    [Tooltip("Cocher pour faire de cet objet un checkpoint")]
    [SerializeField] private bool isCheckpoint = false;

    [Tooltip("Position de respawn")]
    [SerializeField] private Transform spawnPoint;

    protected override void OnInteracted()
    {
        // 1. Activer les objets (avec délai si spécifié)
        if (objectsToActivate != null && objectsToActivate.Length > 0)
        {
            if (delayBeforeActivate > 0)
            {
                StartCoroutine(ActivateObjectsWithDelay(delayBeforeActivate));
            }
            else
            {
                ActivateObjects();
            }
        }

        // 2. Désactiver les objets (avec délai si spécifié)
        if (objectsToDeactivate != null && objectsToDeactivate.Length > 0)
        {
            if (delayBeforeDeactivate > 0)
            {
                StartCoroutine(DeactivateObjectsWithDelay(delayBeforeDeactivate));
            }
            else
            {
                DeactivateObjects();
            }
        }

        // 3. Exécuter l'action complexe si spécifiée
        if (!string.IsNullOrEmpty(actionName))
        {
            if (ActionManager.Instance != null)
            {
                // Utiliser l'objectif s'il est défini, sinon null (utilisera l'objectif de l'action)
                string objectiveToUse = !string.IsNullOrEmpty(objective) ? objective : null;
                ActionManager.Instance.ExecuteAction(actionName, objectiveToUse);
            }
            else
            {
                Debug.LogError("ActionManager n'est pas présent dans la scène!");
            }
        }

        // 4. Enregistrer le checkpoint si activé
        if (isCheckpoint && CheckpointManager.Instance != null)
        {
            if (spawnPoint != null)
            {
                CheckpointManager.Instance.SetSpawnPoint(spawnPoint);
            }
            else
            {
                Debug.LogWarning($"Checkpoint activé sur {gameObject.name} mais aucun Spawn Point n'est assigné!");
            }
        }
    }

    private IEnumerator ActivateObjectsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateObjects();
    }

    private IEnumerator DeactivateObjectsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateObjects();
    }

    private void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);

                // Si c'est un InteractableObject, l'activer aussi dans le GameManager
                InteractableObject interactable = obj.GetComponent<InteractableObject>();
                if (interactable != null && GameManager.Instance != null)
                {
                    GameManager.Instance.AllowInteractable(interactable);
                }
            }
        }
    }

    private void DeactivateObjects()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);

                // Si c'est un InteractableObject, le désactiver aussi dans le GameManager
                InteractableObject interactable = obj.GetComponent<InteractableObject>();
                if (interactable != null && GameManager.Instance != null)
                {
                    GameManager.Instance.DisallowInteractable(interactable);
                }
            }
        }
    }
}
