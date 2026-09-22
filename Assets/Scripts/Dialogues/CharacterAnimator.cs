using UnityEngine;

/// <summary>
/// Gère les animations d'un personnage durant les dialogues.
/// Fonctionne avec un Animator Unity.
/// </summary>
public class CharacterAnimator : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Nom du personnage (doit correspondre au tag 'speaker' dans Ink)")]
    [SerializeField] private string characterName;

    [Header("Références")]
    [Tooltip("Animator du personnage (optionnel si sur le même GameObject)")]
    [SerializeField] private Animator animator;

    [Header("Paramètres d'animation")]
    [Tooltip("Nom du paramètre bool 'IsTalking' dans l'Animator")]
    [SerializeField] private string isTalkingParameter = "IsTalking";

    [Tooltip("Nom du trigger pour démarrer l'animation de parole")]
    [SerializeField] private string talkTrigger = "Talk";

    private void Awake()
    {
        // Si l'animator n'est pas assigné, essayer de le récupérer sur ce GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // S'enregistrer auprès du DialogueAnimationManager dans Start() au lieu de Awake()
        // pour s'assurer que DialogueAnimationManager.Instance est bien initialisé
        if (DialogueAnimationManager.Instance != null)
        {
            DialogueAnimationManager.Instance.RegisterCharacter(characterName, this);
        }
        else
        {
            Debug.LogError($"CharacterAnimator '{characterName}': DialogueAnimationManager.Instance est NULL! Créez un GameObject avec le composant DialogueAnimationManager dans la scène.");
        }
    }

    private void OnDestroy()
    {
        // Se désenregistrer quand l'objet est détruit
        if (DialogueAnimationManager.Instance != null)
        {
            DialogueAnimationManager.Instance.UnregisterCharacter(characterName);
        }
    }

    /// <summary>
    /// Démarre l'animation de parole
    /// </summary>
    public void StartTalking()
    {
        if (animator == null) return;

        // Activer le paramètre bool "IsTalking"
        if (!string.IsNullOrEmpty(isTalkingParameter))
        {
            animator.SetBool(isTalkingParameter, true);
        }

        // Déclencher le trigger "Talk"
        if (!string.IsNullOrEmpty(talkTrigger))
        {
            animator.SetTrigger(talkTrigger);
        }
    }

    /// <summary>
    /// Arrête l'animation de parole
    /// </summary>
    public void StopTalking()
    {
        if (animator == null) return;

        // Désactiver le paramètre bool "IsTalking"
        if (!string.IsNullOrEmpty(isTalkingParameter))
        {
            animator.SetBool(isTalkingParameter, false);
        }
    }

    /// <summary>
    /// Joue une animation spécifique (via trigger)
    /// </summary>
    public void PlayAnimation(string animationTrigger)
    {
        if (animator == null || string.IsNullOrEmpty(animationTrigger)) return;

        animator.SetTrigger(animationTrigger);
    }

    public string GetCharacterName()
    {
        return characterName;
    }
}
