using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        private const ushort GRID_SIZE = 20;
        public Word[] wordsLevelOne;
        //private Word[] wordsLevelTwo;
        private char[,] _solutionGrid;

        private bool _isLevelCompleted;

        private GridCell[,] _cellObjects;

        public RectTransform gridContainer;

        [Tooltip("Prefab for grid cell")]
        public GameObject cellPrefab;

        [Tooltip("Prefab for empty grid cell")]
        public GameObject emptyCellPrefab;

        private Word _selectedWord;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _cellObjects = new GridCell[GRID_SIZE, GRID_SIZE];

            Word[] currentLevelWords = wordsLevelOne;

            InitializeGrids(currentLevelWords);
            DisplayGrid();
            AssociateCellsToWords(currentLevelWords);
        }

        private void InitializeGrids(Word[] wordsLevel)
        {
            _solutionGrid = new char[GRID_SIZE, GRID_SIZE];

            foreach (var data in wordsLevel)
                PlaceWordOnGrid(data, _solutionGrid);
        }

        private void DisplayGrid()
        {
            // Using Y first because of the Grid Layout Group arrangement
            for (ushort y = 0; y < GRID_SIZE; y++)
            {
                for (ushort x = 0; x < GRID_SIZE; x++)
                {
                    if (_solutionGrid[x, y] != '\0')
                    {
                        GameObject cellObj = Instantiate(cellPrefab, gridContainer);
                        GridCell cellScript = cellObj.GetComponent<GridCell>();

                        char correctLetter = _solutionGrid[x, y];

                        cellScript.Initialize(x, y, correctLetter, this);

                        _cellObjects[x, y] = cellScript;
                    }
                    else
                        Instantiate(emptyCellPrefab, gridContainer);
                }
            }
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

            else
                for (int i = 0; i < data.word.Length; i++)
                {
                    grid[x, y] = data.word[i];
                    y++;
                }
        }

        // This fonction allows to associate the GridCell objects to the Word data structure for the highlighting
        private void AssociateCellsToWords(Word[] words)
        {
            foreach (Word data in words)
            {
                int x = data.startX;
                int y = data.startY;

                data.cells ??= new System.Collections.Generic.List<GridCell>();

                //data.cells.Clear();

                if (data.direction == Word.Direction.Horizontal)
                {
                    for (int i = 0; i < data.word.Length; i++)
                    {
                        if (_cellObjects[x, y] != null)
                        {
                            data.cells.Add(_cellObjects[x, y]);
                        }
                        x++;
                    }
                }
                else
                {
                    for (int i = 0; i < data.word.Length; i++)
                    {
                        if (_cellObjects[x, y] != null)
                        {
                            data.cells.Add(_cellObjects[x, y]);
                        }
                        y++;
                    }
                }
            }
        }
    }
}
