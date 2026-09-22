using UnityEngine;

/// <summary>
/// Déclencheur de dialogue : hérite d'InteractableObject pour le déplacement, puis lance un dialogue.
/// </summary>
public class DialogueTrigger : InteractableObject
{
    [Header("Ink JSON")]
    public TextAsset[] inkJsons; // Liste de fichiers JSON
    private int currentJsonIndex; // Index du fichier JSON actuel

    /// <summary>
    /// Appelé automatiquement par InteractableObject quand le joueur interagit.
    /// </summary>
    protected override void OnInteracted()
    {
        StartDialogue();
    }

    /// <summary>
    /// Démarre le dialogue.
    /// </summary>
    /// <param name="waitALittle">Indique s'il faut attendre un peu avant de commencer.</param>
    public void StartDialogue(bool waitALittle = true)
    {

        if (inkJsons == null || inkJsons.Length == 0)
        {
            Debug.LogError("DialogueTrigger: Aucun fichier Ink JSON assigné!");
            return;
        }

        if (currentJsonIndex >= inkJsons.Length)
        {
            Debug.LogError($"DialogueTrigger: Index {currentJsonIndex} hors limites (max: {inkJsons.Length - 1})");
            return;
        }

        DialogueManager.Instance.SetActiveDialogueTrigger(this);
        DialogueManager.Instance.EnterDialogueMode(inkJsons[currentJsonIndex], waitALittle);
    }

    /// <summary>
    /// Passe au prochain fichier JSON.
    /// </summary>
    public void NextJson()
    {
        if (currentJsonIndex < inkJsons.Length - 1)
        {
            currentJsonIndex++; // Passe au prochain fichier JSON seulement si pas dernier
        }
    }

    /// <summary>
    /// Récupère l'index actuel.
    /// </summary>
    /// <returns>L'index actuel.</returns>
    public int GetCurrentJsonIndex()
    {
        return currentJsonIndex;
    }
}
