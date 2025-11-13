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

        public Word(string word, ushort startX, ushort startY, Direction direction, string clue)
        {
            this.word = word;
            this.startX = startX;
            this.startY = startY;
            this.direction = direction;
            this.clue = clue;
        }
    }
}
