using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        private const ushort GRID_SIZE = 20;
        private Word[] _words;
        public Word[] wordsLevelOne;
        //private Word[] wordsLevelTwo;
        private char[,] solutionGrid;
        private char[,] playerGrid;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeGrids();
            //DisplayGrid();
        }

        void InitializeGrids()
        {
            solutionGrid = new char[GRID_SIZE, GRID_SIZE];
            playerGrid = new char[GRID_SIZE, GRID_SIZE];

            foreach (var data in words)
            {
                PlaceWordOnGrid(data, solutionGrid);
            }

            // Initialiser la grille du joueur avec des espaces ou des caractères de masquage
            // (La logique de masquage est souvent gérée par l'UI, pas par le tableau de char)
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
    }
}
