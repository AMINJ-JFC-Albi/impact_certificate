using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [System.Serializable]
    public class Word
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }

        [Tooltip("Word to guess")]
        public string word;
        [Tooltip("Start position X in the solution grid")]
        public ushort startX;
        [Tooltip("Start position Y in the solution grid")]
        public ushort startY;
        [Tooltip("Direction of the word (horizontal / vertical)")]
        public Direction direction;
        [Tooltip("Clue to guess the word)")]
        public string clue;

        [System.NonSerialized]
        public bool isCompleted = false;

        [System.NonSerialized]
        public List<GridCell> cells;

        public void SetSize(bool highlighted)
        {
            foreach (var cell in cells)
                cell.SetSize(highlighted);
        }

        public void ValidateWord()
        {
            isCompleted = true;
            foreach (var cell in cells)
                cell.ValidateCell();
        }
    }
}
