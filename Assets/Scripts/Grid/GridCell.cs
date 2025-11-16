using TMPro;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private TextMeshPro letterText;

    public int X { get; private set; }
    public int Y { get; private set; }

    public void Initialize(int x, int y, char letter, float cellSize)
    {
        X = x;
        Y = y;

        transform.position = new Vector3(x * cellSize, y * cellSize, 0f);

        letterText.text = (letter != '\0') ? letter.ToString() : "";
    }


    public void Highlight(bool isHighlighted)
    {
        GetComponent<SpriteRenderer>().color = isHighlighted ? Color.yellow : Color.white;
    }
}
