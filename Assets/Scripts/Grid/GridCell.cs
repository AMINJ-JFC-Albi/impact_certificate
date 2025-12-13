using TMPro;
using UnityEngine;
using UnityEngine.UI; // IMPORTANT : Ajoutez l'import pour les composants UI

public class GridCell : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Image background;


    public Grid.GridManager manager;

    // Données de position et de lettre
    public int X { get; private set; }
    public int Y { get; private set; }
    public char CorrectLetter { get; private set; }

    public void Initialize(int x, int y, char correctLetter, Grid.GridManager gridManager)
    {
        X = x;
        Y = y;
        CorrectLetter = correctLetter;
        manager = gridManager;


        //inputField.onValueChanged.AddListener(OnLetterEntered);
    }

    // Méthode appelée lors de la saisie
    //private void OnLetterEntered(string letter)
    //{
    //    if (string.IsNullOrEmpty(letter)) return;

    //    //char enteredChar = letter.ToUpper()[0];

    //    // Appel du Manager pour vérifier la lettre
    //    // manager.CheckLetter(this, enteredChar);
    //}

    public void Highlight(bool isHighlighted)
    {
        // Utiliser l'Image (background) au lieu du SpriteRenderer
        background.color = isHighlighted ? Color.yellow : Color.white;

        // Mettre le focus sur le champ de saisie
        if (isHighlighted)
            inputField.Select();
    }

    //public void SetBackground(Color color)
    //{
    //    background.color = color;
    //}

    //public void SetInputInteractable(bool interactable)
    //{
    //    inputField.interactable = interactable;
    //}
}