using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire d'actions déclenchables depuis les dialogues Ink et TriggerAction
/// Gère l'activation/désactivation d'objets et l'exécution de code spécifique
/// </summary>
public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;

    [System.Serializable]
    public class Action
    {
        [Tooltip("Nom de l'action (ex: open_doors_telescope)")]
        public string actionName;

        [Tooltip("Délai avant l'exécution du code spécifique (en secondes)")]
        public float delayBeforeAction = 0f;

        [Tooltip("Objets interactifs à activer (rend l'objet interactif)")]
        public InteractableObject[] interactablesToEnable;

        [Tooltip("Objets interactifs à désactiver (garde l'objet visible mais retire l'interaction)")]
        public InteractableObject[] interactablesToDisable;
    }

    [Header("Actions disponibles")]
    [SerializeField] private List<Action> actions = new List<Action>();

    [Header("Références aux composants")]
    [SerializeField] private OpenDoors openDoorsComponent;

    [Header("Télescope")]
    [SerializeField] private UnityEngine.Playables.PlayableDirector telescopeTimeline;
    [SerializeField] private bool telescopeAutoReturn = true;

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

    /// <summary>
    /// Exécute une action en fonction de son nom
    /// </summary>
    /// <param name="actionName">Nom de l'action à exécuter (ex: "open_doors_telescope")</param>
    public void ExecuteAction(string actionName)
    {
        Action action = actions.Find(a => a.actionName == actionName);

        if (action == null)
        {
            Debug.LogWarning($"Action '{actionName}' non trouvée dans la liste des actions disponibles!");
            return;
        }

        // Désactiver immédiatement les InteractableObjects (pas de délai)
        if (action.interactablesToDisable != null)
        {
            foreach (InteractableObject interactable in action.interactablesToDisable)
            {
                if (interactable != null && GameManager.Instance != null)
                {
                    GameManager.Instance.DisallowInteractable(interactable);
                }
            }
        }

        // Lancer la coroutine qui gère le délai avant d'activer les objets et exécuter le code
        StartCoroutine(ExecuteActionWithDelay(action, actionName));
    }

    /// <summary>
    /// Exécute une action complète avec son délai (activation d'objets + code spécifique)
    /// </summary>
    private IEnumerator ExecuteActionWithDelay(Action action, string actionName)
    {
        // Attendre le délai défini
        if (action.delayBeforeAction > 0)
        {
            yield return new WaitForSeconds(action.delayBeforeAction);
        }

        // Activer les InteractableObjects après le délai (les ajoute à la liste autorisée du GameManager)
        if (action.interactablesToEnable != null)
        {
            foreach (InteractableObject interactable in action.interactablesToEnable)
            {
                if (interactable != null && GameManager.Instance != null)
                {
                    GameManager.Instance.AllowInteractable(interactable);
                }
            }
        }

        // Exécuter le code spécifique de l'action
        ExecuteSpecificAction(actionName);
    }

    /// <summary>
    /// Exécute le code spécifique d'une action
    /// </summary>
    private void ExecuteSpecificAction(string actionName)
    {
        // Switch pour gérer les différentes actions
        switch (actionName)
        {
            case "open_doors_telescope":
                ExecuteOpenDoorsTelescope();
                break;

            case "telescope_action":
                ExecuteTelescopeAction();
                break;

            // Ajouter d'autres actions ici
            // case "autre_action":
            //     ExecuteAutreAction();
            //     break;

            default:
                Debug.Log($"Aucun code spécifique défini pour l'action : {actionName}");
                break;
        }
    }

    #region Actions Spécifiques

    /// <summary>
    /// Action spécifique : Ouvre les 4 premières portes pour accéder au télescope
    /// </summary>
    private void ExecuteOpenDoorsTelescope()
    {
        if (openDoorsComponent != null)
        {
            openDoorsComponent.OpenFourFirstDoors();
        }
    }

    /// <summary>
    /// Action spécifique : Active le télescope (joue la Timeline de zoom caméra)
    /// </summary>
    private void ExecuteTelescopeAction()
    {
        if (telescopeTimeline != null)
        {
            telescopeTimeline.time = 0;
            telescopeTimeline.Play();

            if (telescopeAutoReturn)
            {
                telescopeTimeline.stopped += OnTelescopeTimelineStopped;
            }
        }
        else
        {
            Debug.LogError("Telescope Timeline n'est pas assignée dans l'ActionManager!");
        }
    }

    private void OnTelescopeTimelineStopped(UnityEngine.Playables.PlayableDirector director)
    {
        director.stopped -= OnTelescopeTimelineStopped;
        director.time = 0;
        director.Evaluate();
    }

    private void OnDisable()
    {
        // Nettoyer l'abonnement si l'ActionManager est désactivé
        if (telescopeTimeline != null)
        {
            telescopeTimeline.stopped -= OnTelescopeTimelineStopped;
        }
    }

    // Ajouter d'autres méthodes d'actions spécifiques ici
    // private void ExecuteAutreAction()
    // {
    //     // Code de l'action
    // }

    #endregion
}
