using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float BIGGER_CELL = 40f;
    private const float NORMAL_CELL = 30f;

    public TMP_InputField inputField;

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
    public bool filled;



    public void Start()
    {
        inputField.onValueChanged.AddListener(delegate { ValueChanged(); });
    }

    public void Initialize(int x, int y, char correctLetter, Grid.GridManager gridManager)
    {
        X = x;
        Y = y;
        CorrectLetter = correctLetter;
        manager = gridManager;
        rt = inputStyle.GetComponent<RectTransform>();
        filled = false;

        //inputField.onValueChanged.AddListener(OnLetterEntered);
    }

    #region Highlight on hover

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
            rt.sizeDelta = new Vector2(BIGGER_CELL, BIGGER_CELL); // replace to a constant
        else
            rt.sizeDelta = new Vector2(NORMAL_CELL, NORMAL_CELL);
    }

    #endregion

    public void ValueChanged()
    {
        //Debug.Log("Value Changed");
        if (inputField.text.Length > 0)
        {
            filled = true;
            manager.OnCellValueChanged(this);
        }
        else
            filled = false;
    }
}