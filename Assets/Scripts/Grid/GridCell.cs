using Grid;
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

}