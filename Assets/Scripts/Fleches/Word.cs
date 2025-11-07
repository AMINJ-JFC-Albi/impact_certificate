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

        public string word;
        public ushort startX;
        public ushort startY;
        public Direction direction;
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
