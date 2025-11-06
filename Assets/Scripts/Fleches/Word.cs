using UnityEngine;

[System.Serializable]
public class Word
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public string word;
    public int startX;
    public int startY;
    public Direction direction;
    public string clue;
}
