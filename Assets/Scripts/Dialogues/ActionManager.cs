using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gestionnaire d'actions complexes et actions partagées (utilisées par dialogues et triggers)
/// Les actions simples non-partagées peuvent être gérées directement par ActionTrigger
/// </summary>
public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;

    [System.Serializable]
    public class Action
    {
        [Tooltip("Nom de l'action")]
        public string actionName;

        [Tooltip("Délai avant l'exécution de l'action")]
        public float delayBeforeAction = 0f;

        [Tooltip("Objets à activer")]
        public GameObject[] objectsToActivate;

        [Tooltip("Objets à désactiver")]
        public GameObject[] objectsToDeactivate;
    }

    [Header("Actions partagées (dialogues + triggers)")]
    [Tooltip("Actions pouvant être appelées depuis les dialogues Ink ou les triggers")]
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

    private void OnDisable()
    {
        // Nettoyer l'abonnement si l'ActionManager est désactivé
        if (telescopeTimeline != null)
        {
            telescopeTimeline.stopped -= OnTelescopeTimelineStopped;
        }
    }

    /// <summary>
    /// Exécute une action en fonction de son nom
    /// </summary>
    /// <param name="actionName">Nom de l'action à exécuter </param>
    public void ExecuteAction(string actionName)
    {
        // Chercher l'action dans la liste
        Action action = actions.Find(a => a.actionName == actionName);

        if (action != null)
        {
            // Si délai, utiliser une coroutine
            if (action.delayBeforeAction > 0)
            {
                StartCoroutine(ExecuteActionWithDelay(action, actionName));
            }
            else
            {
                // Exécution immédiate
                ExecuteActionObjects(action);
                ExecuteSpecificAction(actionName);
            }
        }
        else
        {
            // Pas d'action dans la liste, essayer juste le code spécifique
            ExecuteSpecificAction(actionName);
        }
    }

    /// <summary>
    /// Exécute une action avec un délai
    /// </summary>
    private IEnumerator ExecuteActionWithDelay(Action action, string actionName)
    {
        yield return new WaitForSeconds(action.delayBeforeAction);
        ExecuteActionObjects(action);
        ExecuteSpecificAction(actionName);
    }

    /// <summary>
    /// Active/désactive les objets de l'action
    /// </summary>
    private void ExecuteActionObjects(Action action)
    {
        // Activer les objets
        if (action.objectsToActivate != null)
        {
            foreach (GameObject obj in action.objectsToActivate)
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

        // Désactiver les objets
        if (action.objectsToDeactivate != null)
        {
            foreach (GameObject obj in action.objectsToDeactivate)
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

            case "reset_on_death":
                ExecuteResetOnDeath();
                break;

            case "Start_phase_infiltration":
                GameManager.Instance.SetInfiltrationMode(true);
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



    /// <summary>
    /// Action spécifique : Réinitialise le joueur au checkpoint actuel après la mort
    /// </summary>
    private void ExecuteResetOnDeath()
    {
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }
        else
        {
            Debug.LogError("CheckpointManager introuvable! Impossible de respawn le joueur.");
        }


    }
    #endregion
}
