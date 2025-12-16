using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float BIGGER_CELL = 40f;
    private const float NORMAL_CELL = 30f;

    public TMP_InputField inputField;

    [SerializeField]
    private GameObject inputStyle;
    private RectTransform rt;

    [NonSerialized]
    public Grid.GridManager manager;

    // Données de position et de lettre
    public int X { get; private set; }
    public int Y { get; private set; }
    public char CorrectLetter { get; private set; }

    [NonSerialized]
    public bool filled;
    [NonSerialized]
    public bool isValidated;



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
        isValidated = false;
    }

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

    public void Clear()
    {
        if (!isValidated)
        {
            inputField.text = string.Empty;
            filled = false;
        }
    }

    public void ValidateCell()
    {
        isValidated = true;
        inputField.interactable = false;
    }
}