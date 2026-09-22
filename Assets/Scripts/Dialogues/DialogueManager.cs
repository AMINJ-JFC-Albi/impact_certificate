using System.Collections;
using System.Collections.Generic;

using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère les dialogues dans le jeu.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject speakerNameObject; // Conteneur du nom du locuteur
    [SerializeField] private TextMeshProUGUI speakerNameText; // Texte du nom du locuteur

    [Header("Choice UI")][SerializeField] private GameObject[] choices;

    private TextMeshProUGUI[] choicesText;
    private List<Choice> currentChoices;
    private RectTransform onlyChoice;
    private RectTransform secondeChoice;

    private Story currentStory;

    private DialogueTrigger activeDialogueTrigger;

    [Header("Dialogue Pass Button")]
    [SerializeField]
    private Button canPass;
    [SerializeField] private GameObject arrow;

    private CanvasRenderer arrowRenderer;
    private Coroutine typeCoroutine;
    private bool inTypeCoroutine;
    private float pendingDelay = 0f; // Délai en attente après le prochain clic


    /// <summary>
    /// Initialise les composants nécessaires.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CanPassDialogue(true);
        dialoguePanel = transform.GetChild(0).gameObject;

        // Récupérer le texte pour chaque choix
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            choice.gameObject.SetActive(false);
            index++;
        }

        onlyChoice = choices[0].GetComponent<RectTransform>();
        secondeChoice = choices[1].GetComponent<RectTransform>();

        arrowRenderer = arrow.GetComponent<CanvasRenderer>();
    }

    /// <summary>
    /// Met à jour l'état du dialogue chaque frame.
    /// </summary>
    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * 3) + 1) / 2;
        arrowRenderer.SetAlpha(alpha);
    }

    /// <summary>
    /// Active ou désactive le bouton de passage de dialogue.
    /// </summary>
    /// <param name="active">Indique si le bouton doit être actif.</param>
    public void CanPassDialogue(bool active)
    {
        canPass.enabled = active;
        arrow.SetActive(active);
    }

    /// <summary>
    /// Définit le déclencheur de dialogue actif.
    /// </summary>
    /// <param name="trigger">Le déclencheur de dialogue.</param>
    public void SetActiveDialogueTrigger(DialogueTrigger trigger)
    {
        activeDialogueTrigger = trigger;
    }

    /// <summary>
    /// Entre en mode dialogue.
    /// </summary>
    /// <param name="inkJson">Le fichier JSON Ink.</param>
    /// <param name="waitALittle">Indique s'il faut attendre un peu avant de commencer.</param>
    public void EnterDialogueMode(TextAsset inkJson, bool waitALittle = true)
    {
        // Désactiver toutes les interactions dans le jeu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeInteractionMode(false);
        }

        if (waitALittle)
        {
            CanPassDialogue(false);
            StartCoroutine(WaitALittleBit());
        }

        currentStory = new Story(inkJson.text);
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    /// <summary>
    /// Coroutine pour attendre un peu avant de permettre le passage de dialogue.
    /// </summary>
    private IEnumerator WaitALittleBit()
    {
        yield return new WaitForSeconds(0.1f);
        CanPassDialogue(true);
    }

    /// <summary>
    /// Quitte le mode dialogue.
    /// </summary>
    private void ExitDialogueMode()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        // Arrêter toutes les animations de dialogue
        if (DialogueAnimationManager.Instance != null)
        {
            DialogueAnimationManager.Instance.StopAllSpeaking();
        }

        // Réactiver toutes les interactions dans le jeu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeInteractionMode(true);
        }

        if (activeDialogueTrigger)
        {
            activeDialogueTrigger.NextJson(); // Réinitialiser si nécessaire
            activeDialogueTrigger = null; // Libérer la référence
        }
        currentStory = null;
    }

    /// <summary>
    /// Continue l'histoire du dialogue.
    /// </summary>
    public void ContinueStory()
    {
        // Si un delay est en attente, l'exécuter d'abord
        if (pendingDelay > 0f)
        {
            StartCoroutine(DelayThenContinue(pendingDelay));
            pendingDelay = 0f;
            return;
        }

        // Si l'histoire peut continuer avec du texte
        if (inTypeCoroutine)
        {
            StopCoroutine(typeCoroutine);
            dialogueText.text = currentStory.currentText;
            DisplayChoices();
            inTypeCoroutine = false;
        }
        else if (currentStory.canContinue)
        {
            foreach (GameObject t in choices)
            {
                t.gameObject.SetActive(false);
            }
            string text = currentStory.Continue();

            // Gérer les tags 
            HandleTags(currentStory.currentTags);

            typeCoroutine = StartCoroutine(TypeSentence(text));
            currentChoices = currentStory.currentChoices;
        }
        else
        {
            // Si l'histoire ne peut plus continuer, mais qu'il y a encore des choix
            if (currentStory.currentChoices.Count == 0)
            {
                // Si aucun choix n'est disponible et que l'histoire est terminée, quitter le dialogue
                ExitDialogueMode();

            }
            else if (currentStory.currentChoices.Count == 1)
            {
                // S'il n'y a qu'un choix, le choisir par défaut
                MakeChoice(0);
            }


        }
    }

    /// <summary>
    /// Coroutine pour taper une phrase lettre par lettre.
    /// </summary>
    private IEnumerator TypeSentence(string sentence)
    {
        inTypeCoroutine = true;
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f); // Ajustez la vitesse de frappe ici
        }
        DisplayChoices(); // Affiche les choix si présents
        inTypeCoroutine = false;
    }




    /// <summary>
    /// Affiche les choix de dialogue.
    /// </summary>
    private void DisplayChoices()
    {
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Plus de choix que ce que peut contenir l'UI" + currentChoices.Count);
            return;
        }

        if (currentChoices.Count > 1)
        {
            CanPassDialogue(false);
        }

        int index = 0;
        // Enable le nombre d'UI en fonction du nombre de choix possible
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        // Si moins de choix dans le INK que dans le nombre de Choix de l'UI, enlever les UI en trop
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        // S'il n'y a qu'un seul choix, l'agrandir pour qu'il prenne tout l'espace
        switch (currentChoices.Count)
        {
            case 1:
                onlyChoice.localPosition = new Vector3(0, onlyChoice.localPosition.y, onlyChoice.localPosition.z);
                onlyChoice.sizeDelta = new Vector2(450, 60);
                break;
            case 2:
                onlyChoice.localPosition = new Vector3(-300, onlyChoice.localPosition.y, onlyChoice.localPosition.z);
                onlyChoice.sizeDelta = new Vector2(200, 60);

                secondeChoice.localPosition = new Vector3(300, secondeChoice.localPosition.y, secondeChoice.localPosition.z);
                secondeChoice.sizeDelta = new Vector2(200, 60);
                break;
            default:
                onlyChoice.localPosition = new Vector3(-400, onlyChoice.localPosition.y, onlyChoice.localPosition.z);
                onlyChoice.sizeDelta = new Vector2(150, 60);

                secondeChoice.localPosition = new Vector3(0, secondeChoice.localPosition.y, secondeChoice.localPosition.z);
                secondeChoice.sizeDelta = new Vector2(150, 60);
                break;
        }
    }

    /// <summary>
    /// Fait un choix de dialogue.
    /// </summary>
    /// <param name="choiceIndex">L'index du choix.</param>
    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        CanPassDialogue(true);
        ContinueStory();
    }

    /// <summary>
    /// Gère les tags Ink pour extraire le nom du locuteur.
    /// </summary>
    /// <param name="tags">Liste des tags de la ligne actuelle</param>
    private void HandleTags(List<string> tags)
    {
        bool speakerFound = false;

        // Parcourir tous les tags
        foreach (string tag in tags)
        {
            // Séparer le tag en clé:valeur
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogWarning($"Tag mal formaté: {tag}");
                continue;
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // Gérer le tag "speaker"
            if (tagKey == "speaker")
            {
                speakerFound = true;
                if (speakerNameObject != null)
                {
                    speakerNameObject.SetActive(true); // Afficher le conteneur
                }
                if (speakerNameText != null)
                {
                    speakerNameText.text = tagValue;
                }
                else
                {
                    Debug.LogWarning("speakerNameText n'est pas assigné dans l'Inspector!");
                }

                // Déclencher l'animation du personnage qui parle
                if (DialogueAnimationManager.Instance != null)
                {
                    DialogueAnimationManager.Instance.SetSpeaker(tagValue);
                }
            }
            // Gérer le tag "action"
            else if (tagKey == "action")
            {
                if (ActionManager.Instance != null)
                {
                    ActionManager.Instance.ExecuteAction(tagValue);
                }
                else
                {
                    Debug.LogWarning("ActionManager n'est pas présent dans la scène!");
                }
            }
            // Gérer le tag "delay" pour une pause automatique
            else if (tagKey == "delay")
            {
                if (float.TryParse(tagValue, out float delaySeconds))
                {
                    pendingDelay = delaySeconds; // Stocker pour après le clic
                }
                else
                {
                    Debug.LogWarning($"Valeur de delay invalide: {tagValue}");
                }
            }

        }

        // Si aucun tag "speaker" trouvé, cacher le GameObject
        if (!speakerFound && speakerNameObject != null)
        {
            speakerNameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine pour attendre un délai puis continuer le dialogue automatiquement
    /// </summary>
    private IEnumerator DelayThenContinue(float seconds)
    {
        CanPassDialogue(false);
        dialoguePanel.SetActive(false); // Masquer le dialogue pendant l'attente

        // Arrêter les animations de dialogue
        if (DialogueAnimationManager.Instance != null)
        {
            DialogueAnimationManager.Instance.StopAllSpeaking();
        }

        yield return new WaitForSeconds(seconds);
        dialoguePanel.SetActive(true); // Réafficher le dialogue
        CanPassDialogue(true);
        ContinueStory();
    }
}