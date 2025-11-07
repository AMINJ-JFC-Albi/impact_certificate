using UnityEngine;

/// <summary>
/// Déclenche une action du ActionManager quand on clique sur l'objet.
/// </summary>
public class ActionTrigger : InteractableObject
{
    [Header("Action à déclencher")]
    [Tooltip("Nom de l'action à exécuter dans l'ActionManager (ex: telescope_action)")]
    [SerializeField] private string actionName = "telescope_action";

    protected override void OnInteracted()
    {
        if (ActionManager.Instance != null)
        {
            ActionManager.Instance.ExecuteAction(actionName);
        }
        else
        {
            Debug.LogError("ActionManager n'est pas présent dans la scène!");
        }
    }
}
