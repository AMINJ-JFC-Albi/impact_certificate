using UnityEngine;

/// <summary>
/// Déclencheur de dialogue : hérite d'InteractableObject pour le déplacement, puis lance un dialogue.
/// </summary>
public class DialogueTrigger : InteractableObject
{

    [Header("Ink JSON")]
    public TextAsset[] inkJsons; // Liste de fichiers JSON
    private int currentJsonIndex; // Index du fichier JSON actuel

    private PlayerControl.PlayerController player;
    private bool isPlayerMovingToThis = false;

    protected override void Start()
    {
        base.Start(); // Appelle l'initialisation de la classe parente
        player = Object.FindFirstObjectByType<PlayerControl.PlayerController>();
    }

    protected override void Update()
    {
        // Appeler l'Update de la classe parente pour le raycast/clic/hover
        base.Update();

        // Vérifier si le joueur est arrivé à destination
        if (isPlayerMovingToThis && player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool isMoving = player.IsMoving();

            // Utiliser interactionDistance + une marge de tolérance pour le déclenchement du dialogue
            float triggerDistance = interactionDistance + 0.5f;

            if (distance <= triggerDistance && !isMoving)
            {
                isPlayerMovingToThis = false;
                StartDialogue();
            }
        }
    }

    protected override void OnClicked()
    {

        if (player == null)
        {
            Debug.LogError("DialogueTrigger: Aucun PlayerController trouvé!");
            return;
        }

        // Vérifier si le joueur est déjà à proximité
        float currentDistance = Vector3.Distance(player.transform.position, transform.position);
        float triggerDistance = interactionDistance + 0.5f;


        if (currentDistance <= triggerDistance)
        {
            // Le joueur est déjà assez proche, lancer le dialogue immédiatement
            StartDialogue();
        }
        else
        {
            // Appeler la méthode de la classe parente pour déplacer le joueur
            base.OnClicked();
            isPlayerMovingToThis = true;

        }
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
