using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float BIGGER_CELL = 54f;
    private const float NORMAL_CELL = 50f;

    public TMP_InputField inputField;
    public GameObject text;

    [SerializeField]
    private GameObject inputStyle;
    private RectTransform rt;

    [NonSerialized]
    public Grid.GridManager manager;

    // Données de position et de lettre
    public int X { get; private set; }
    public int Y { get; private set; }
    public char CorrectLetter { get; private set; }

    public Color32 correctColor = new(0, 255, 138, 255);


    [NonSerialized]
    public bool filled;
    [NonSerialized]
    public bool isValidated;


    #region Initialization

    public void Initialize(int x, int y, char correctLetter, Grid.GridManager gridManager)
    {
        X = x;
        Y = y;
        CorrectLetter = correctLetter;
        manager = gridManager;
        rt = inputStyle.GetComponent<RectTransform>();
        filled = false;
        isValidated = false;
        rt.sizeDelta = new Vector2(NORMAL_CELL, NORMAL_CELL);

        // Value change event
        if (inputField != null)
            inputField.onValueChanged.AddListener(delegate { ValueChanged(); });
    }
    #endregion

    #region Keys

    public void Update()
    {
        if (manager == null || inputField == null)
            return;

        // Ne traiter les flèches que si ce champ a le focus (une seule cellule active répond)
        if (!inputField.isFocused)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            manager.OnCellMoveRequested(this, Grid.GridManager.MoveDirection.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            manager.OnCellMoveRequested(this, Grid.GridManager.MoveDirection.Down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            manager.OnCellMoveRequested(this, Grid.GridManager.MoveDirection.Left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            manager.OnCellMoveRequested(this, Grid.GridManager.MoveDirection.Right);
    }

    #endregion

    #region Highlight on hover

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isValidated)
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

    #region Input Handling
    public void ValueChanged()
    {
        //Debug.Log("Value Changed");
        if (inputField.text.Length > 0)
            filled = true;
        else
            filled = false;
        manager.OnCellValueChanged(this, filled);
    }

    public void Clear()
    {
        if (!isValidated)
        {
            inputField.SetTextWithoutNotify(string.Empty);
            filled = false;
        }
    }

    public void ValidateCell()
    {
        isValidated = true;
        filled = true;
        inputField.interactable = false;
        text.GetComponent<TMP_Text>().color = correctColor;
    }

    #endregion

    #region Fill
    public void Fill(bool checkValidation = true)
    {
        inputField.SetTextWithoutNotify(CorrectLetter.ToString());
        ValidateCell();

        if (checkValidation)
            manager.OnCellValueChanged(this, filled);
    }

    #endregion
}