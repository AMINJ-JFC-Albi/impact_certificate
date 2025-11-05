using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Déclenche une animation de caméra (zoom/focus) quand on interagit avec un télescope.
/// Utilise PlayableDirector (Timeline) pour les animations cinématiques.
/// </summary>
public class TelescopeTrigger : InteractableObject
{
    [Header("Camera Timeline")]
    [SerializeField] private PlayableDirector playableDirector;

    [Header("Auto Return")]
    [SerializeField] private bool autoReturnToStart = true;

    protected override void OnInteracted()
    {
        Debug.Log($"Télescope activé!");

        if (playableDirector != null)
        {
            // Jouer la timeline depuis le début
            playableDirector.time = 0;
            playableDirector.Play();

            // Si auto return, revenir au début après la fin
            if (autoReturnToStart)
            {
                // S'abonner à l'événement de fin de timeline
                playableDirector.stopped += OnTimelineStopped;
            }
        }
        else
        {
            Debug.LogWarning("Playable Director n'est pas assigné!");
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        // Se désabonner pour éviter les appels multiples
        director.stopped -= OnTimelineStopped;

        // Revenir au début de la timeline (frame 0)
        director.time = 0;
        director.Evaluate();
    }

    private void OnDisable()
    {
        // Nettoyer l'abonnement si l'objet est désactivé
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineStopped;
        }
    }
}
