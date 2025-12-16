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

        public Word(string word, ushort startX, ushort startY, Direction direction, string clue)
        {
            this.word = word;
            this.startX = startX;
            this.startY = startY;
            this.direction = direction;
            this.clue = clue;
        }


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


        /*public bool IsOccuped(int x, int y)
        {
            if (direction == Direction.Horizontal)
            {
                if (y != startY)
                    return false;
                return x >= startX && x < startX + (word?.Length ?? 0);
            }

            if (x != startX)
                return false;
            return y >= startY && y < startY + (word?.Length ?? 0);
        }*/
    }
}
