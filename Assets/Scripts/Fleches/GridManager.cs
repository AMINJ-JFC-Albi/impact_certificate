using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        private const ushort GRID_SIZE = 20;
        public Word[] wordsLevelOne;
        //private Word[] wordsLevelTwo;
        private char[,] solutionGrid;
        private char[,] playerGrid;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeGrids(wordsLevelOne);
            //DisplayGrid();
        }

        void InitializeGrids(Word[] wordsLevel)
        {
            solutionGrid = new char[GRID_SIZE, GRID_SIZE];
            playerGrid = new char[GRID_SIZE, GRID_SIZE];

            foreach (var data in wordsLevel)
                PlaceWordOnGrid(data, solutionGrid);
        }


        private void PlaceWordOnGrid(Word data, char[,] grid)
        {
            int x = data.startX;
            int y = data.startY;

            if (data.direction == Word.Direction.Horizontal)
                for (int i = 0; i < data.word.Length; i++)
                {
                    grid[x, y] = data.word[i];
                    x++;
                }

            else // Vertical
                for (int i = 0; i < data.word.Length; i++)
                {
                    grid[x, y] = data.word[i];
                    y++;
                }
        }

        private void DisplayGrid()
        {

        }
    }
}
