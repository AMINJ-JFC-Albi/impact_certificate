using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire central des animations de personnages durant les dialogues.
/// Lie les noms de personnages (tags Ink) aux CharacterAnimator.
/// </summary>
public class DialogueAnimationManager : MonoBehaviour
{
    public static DialogueAnimationManager Instance;

    // Dictionnaire : nom du personnage -> CharacterAnimator
    private Dictionary<string, CharacterAnimator> characterAnimators = new Dictionary<string, CharacterAnimator>();

    // Personnage actuellement en train de parler
    private CharacterAnimator currentSpeaker;

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
    /// Enregistre un personnage dans le dictionnaire
    /// </summary>
    public void RegisterCharacter(string characterName, CharacterAnimator animator)
    {
        if (string.IsNullOrEmpty(characterName))
        {
            Debug.LogWarning("Tentative d'enregistrement d'un personnage sans nom!");
            return;
        }

        if (characterAnimators.ContainsKey(characterName))
        {
            characterAnimators[characterName] = animator;
        }
        else
        {
            characterAnimators.Add(characterName, animator);
        }
    }

    /// <summary>
    /// Désenregistre un personnage du dictionnaire
    /// </summary>
    public void UnregisterCharacter(string characterName)
    {
        if (characterAnimators.ContainsKey(characterName))
        {
            characterAnimators.Remove(characterName);
        }
    }

    /// <summary>
    /// Change le personnage qui parle (arrête l'ancien, démarre le nouveau)
    /// </summary>
    public void SetSpeaker(string speakerName)
    {
        // Arrêter l'animation du locuteur précédent
        if (currentSpeaker != null)
        {
            currentSpeaker.StopTalking();
        }

        // Démarrer l'animation du nouveau locuteur
        if (characterAnimators.TryGetValue(speakerName, out CharacterAnimator newSpeaker))
        {
            currentSpeaker = newSpeaker;
            currentSpeaker.StartTalking();
        }
        else
        {
            currentSpeaker = null;
            Debug.LogWarning($"Aucun CharacterAnimator trouvé pour '{speakerName}'. Vérifiez que le personnage est bien enregistré.");
        }
    }

    /// <summary>
    /// Arrête toutes les animations de dialogue
    /// </summary>
    public void StopAllSpeaking()
    {
        if (currentSpeaker != null)
        {
            currentSpeaker.StopTalking();
            currentSpeaker = null;
        }
    }

    /// <summary>
    /// Joue une animation spécifique sur un personnage
    /// </summary>
    public void PlayCharacterAnimation(string characterName, string animationTrigger)
    {
        if (characterAnimators.TryGetValue(characterName, out CharacterAnimator animator))
        {
            animator.PlayAnimation(animationTrigger);
        }
        else
        {
            Debug.LogWarning($"Personnage '{characterName}' non trouvé pour jouer l'animation '{animationTrigger}'");
        }
    }
}
