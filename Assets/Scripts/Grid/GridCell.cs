using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private GameObject inputStyle;
    private RectTransform rt;

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
        rt = inputStyle.GetComponent<RectTransform>();

        //inputField.onValueChanged.AddListener(OnLetterEntered);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.OnCellHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.OnCellUnhovered(this);
    }

    public void SetSize(bool sized)
    {
        if (sized)
            rt.sizeDelta = new Vector2(40f, 40f);
        else
            rt.sizeDelta = new Vector2(30f, 30f);

        // set height and width to 40



    }
}