using UnityEngine;
using TMPro;

public class ClueTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private readonly Vector2 OFFSET = new(15f, -15f);

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false); // Caché par défaut
    }

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.worldCamera,
            out Vector2 localPoint);

        float tooltipWidth = rectTransform.rect.width;
        float tooltipHeight = rectTransform.rect.height;
        Rect canvasRect = (parentCanvas.transform as RectTransform).rect;

        Vector2 finalOffset = OFFSET;


        if (localPoint.x + OFFSET.x + tooltipWidth > canvasRect.xMax)
            finalOffset.x = -tooltipWidth - Mathf.Abs(OFFSET.x);

        if (localPoint.y + OFFSET.y - tooltipHeight < canvasRect.yMin)
            finalOffset.y = tooltipHeight + Mathf.Abs(OFFSET.y);

        rectTransform.anchoredPosition = localPoint + finalOffset;
    }

    public void ShowClue(string message)
    {
        textComponent.text = message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}