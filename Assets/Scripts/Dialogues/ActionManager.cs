using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

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

        [Tooltip("Objectif associé à cette action (optionnel)")]
        public string objective = "";

        [Tooltip("Objets à activer")]
        public GameObject[] objectsToActivate;

        [Tooltip("Délai avant d'activer les objets (0 = immédiat)")]
        public float delayBeforeActivate = 0f;

        [Tooltip("Objets à désactiver")]
        public GameObject[] objectsToDeactivate;

        [Tooltip("Délai avant de désactiver les objets (0 = immédiat)")]
        public float delayBeforeDeactivate = 0f;

        [Tooltip("Objets dont on active uniquement l'interaction")]
        public InteractableObject[] interactionsToEnable;

        [Tooltip("Objets dont on désactive uniquement l'interaction ")]
        public InteractableObject[] interactionsToDisable;

        [Tooltip("Event")]
        public UnityEvent unityEvent;
    }

    [Header("Actions partagées (dialogues + triggers)")]
    [Tooltip("Actions pouvant être appelées depuis les dialogues Ink ou les triggers")]
    [SerializeField] private List<Action> actions = new List<Action>();

    [Header("Références aux composants")]
    [SerializeField] private OpenDoors openDoorsComponent;

    [System.Serializable]
    public class TimelineAction
    {
        public string name;
        public UnityEngine.Playables.PlayableAsset timeline;
        public bool autoReset = true;
    }

    [Header("Timelines")]
    [SerializeField] private UnityEngine.Playables.PlayableDirector sharedDirector;
    [SerializeField] private TimelineAction[] timelineActions;

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
        // Nettoyer les abonnements si l'ActionManager est désactivé
        if (sharedDirector != null)
        {
            sharedDirector.stopped -= OnTimelineStopped;
        }
    }

    /// <summary>
    /// Exécute une action en fonction de son nom
    /// </summary>
    /// <param name="actionName">Nom de l'action à exécuter </param>
    public void ExecuteAction(string actionName)
    {
        ExecuteAction(actionName, null);
    }

    /// <summary>
    /// Exécute une action en fonction de son nom avec un objectif personnalisé
    /// </summary>
    /// <param name="actionName">Nom de l'action à exécuter </param>
    /// <param name="customObjective">Objectif personnalisé (si null, utilise l'objectif de l'action)</param>
    public void ExecuteAction(string actionName, string customObjective)
    {
        // Chercher l'action dans la liste
        Action action = actions.Find(a => a.actionName == actionName);

        if (action != null)
        {
            // Déterminer quel objectif utiliser
            string objectiveToShow = customObjective ?? action.objective;

            // Ne changer l'objectif que s'il n'est pas vide (sinon on garde l'actuel)
            if (!string.IsNullOrEmpty(objectiveToShow) && ObjectiveUI.Instance != null)
            {
                ObjectiveUI.Instance.ShowObjective(objectiveToShow);
            }

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
            // Ne changer l'objectif que s'il n'est pas vide (sinon on garde l'actuel)
            if (!string.IsNullOrEmpty(customObjective) && ObjectiveUI.Instance != null)
            {
                ObjectiveUI.Instance.ShowObjective(customObjective);
            }

            // Essayer juste le code spécifique
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
        // Activer les objets (avec délai si spécifié)
        if (action.objectsToActivate != null && action.objectsToActivate.Length > 0)
        {
            if (action.delayBeforeActivate > 0)
            {
                StartCoroutine(ActivateObjectsWithDelay(action.objectsToActivate, action.delayBeforeActivate));
            }
            else
            {
                ActivateObjects(action.objectsToActivate);
            }
        }

        // Désactiver les objets (avec délai si spécifié)
        if (action.objectsToDeactivate != null && action.objectsToDeactivate.Length > 0)
        {
            if (action.delayBeforeDeactivate > 0)
            {
                StartCoroutine(DeactivateObjectsWithDelay(action.objectsToDeactivate, action.delayBeforeDeactivate));
            }
            else
            {
                DeactivateObjects(action.objectsToDeactivate);
            }
        }

        // Activer uniquement l'interaction 
        if (action.interactionsToEnable != null)
        {
            foreach (InteractableObject interactable in action.interactionsToEnable)
            {
                if (interactable != null && GameManager.Instance != null)
                {
                    GameManager.Instance.AllowInteractable(interactable);
                }
            }
        }

        // Désactiver uniquement l'interaction 
        if (action.interactionsToDisable != null)
        {
            foreach (InteractableObject interactable in action.interactionsToDisable)
            {
                if (interactable != null && GameManager.Instance != null)
                {
                    GameManager.Instance.DisallowInteractable(interactable);
                }
            }
        }
    }

    private IEnumerator ActivateObjectsWithDelay(GameObject[] objects, float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateObjects(objects);
    }

    private IEnumerator DeactivateObjectsWithDelay(GameObject[] objects, float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateObjects(objects);
    }

    private void ActivateObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                // Note: On n'active plus automatiquement l'interaction ici
                // Utiliser interactionsToEnable si besoin d'activer l'interaction
            }
        }
    }

    private void DeactivateObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
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
                PlayTimeline("telescope");
                break;

            case "reset_on_death":
                ExecuteResetOnDeath();
                break;

            case "Start_phase_infiltration":
                GameManager.Instance.SetInfiltrationMode(true);
                break;

            // Actions pour afficher les fiches de conception
            case "show_fiche_casqueVR":
                ExecuteShowFiche(0);
                break;

            case "show_fiche_ControleurVR":
                ExecuteShowFiche(1);
                break;

            case "show_fiche_gantsHaptique":
                ExecuteShowFiche(2);
                break;

            case "show_fiche_fullBodyTracking":
                ExecuteShowFiche(3);
                break;

            case "show_fiche_PlateformeOmni":
                ExecuteShowFiche(4);
                break;

            case "show_fiche_casqueAudio":
                ExecuteShowFiche(5);
                break;

            case "hide_fiche":
                ExecuteHideFiche();
                break;
            case "students_enter":
                actions[1].unityEvent.Invoke();
                break;
            case "student_helmet":
                actions[2].unityEvent.Invoke();
                break;
            case "student_help":
                actions[3].unityEvent.Invoke();
                break;
            case "end_ethan_dialog":
                actions[4].unityEvent.Invoke();
                break;
            case "end_alice_dialog":
                actions[5].unityEvent.Invoke();
                break;

            case "return_to_navet":
                PlayTimeline("return_to_navet");
                break;

            case "teleport_to_mission2":
                PlayTimeline("teleport_to_mission2");
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
    /// Joue une timeline par son nom (défini dans timelineActions)
    /// </summary>
    private void PlayTimeline(string timelineName)
    {
        if (timelineActions == null || sharedDirector == null)
        {
            Debug.LogError("TimelineActions ou SharedDirector non assigné dans l'ActionManager!");
            return;
        }

        foreach (var ta in timelineActions)
        {
            if (ta.name == timelineName)
            {
                if (ta.timeline != null)
                {
                    // Assigner la nouvelle timeline au director partagé
                    sharedDirector.playableAsset = ta.timeline;
                    sharedDirector.time = 0;
                    sharedDirector.Play();

                    if (ta.autoReset)
                    {
                        sharedDirector.stopped += OnTimelineStopped;
                    }
                }
                else
                {
                    Debug.LogError($"Timeline '{timelineName}' n'est pas assignée dans l'ActionManager!");
                }
                return;
            }
        }
        Debug.LogWarning($"Timeline '{timelineName}' non trouvée dans timelineActions");
    }

    private void OnTimelineStopped(UnityEngine.Playables.PlayableDirector director)
    {
        director.stopped -= OnTimelineStopped;
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

    /// <summary>
    /// Action spécifique : Affiche une fiche de conception
    /// </summary>
    private void ExecuteShowFiche(int productIndex)
    {
        if (ConceptualSpecificationDesign.Instance != null)
        {
            ConceptualSpecificationDesign.Instance.ShowFiche(productIndex);
            Debug.Log($"Affichage de la fiche de conception pour le produit index {productIndex}");
        }
        else
        {
            Debug.LogError("ConceptualSpecificationDesign introuvable!");
        }
    }

    /// <summary>
    /// Action spécifique : Cache la fiche de conception
    /// </summary>
    private void ExecuteHideFiche()
    {
        if (ConceptualSpecificationDesign.Instance != null)
        {
            ConceptualSpecificationDesign.Instance.HideFiche();
        }
    }
    #endregion
}
