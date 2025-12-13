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



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeGrids(wordsLevelOne);

            _cellObjects = new GridCell[GRID_SIZE, GRID_SIZE];
            DisplayGrid();
        }

        void InitializeGrids(Word[] wordsLevel)
        {
            _solutionGrid = new char[GRID_SIZE, GRID_SIZE];

            foreach (var data in wordsLevel)
                PlaceWordOnGrid(data, _solutionGrid);
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

        private void DisplayGrid()
        {
            for (ushort y = 0; y < GRID_SIZE; y++)
            {
                for (ushort x = 0; x < GRID_SIZE; x++)
                {
                    if (_solutionGrid[x, y] != '\0')
                    {
                        GameObject cellObj = Instantiate(cellPrefab, gridContainer);
                        GridCell cellScript = cellObj.GetComponent<GridCell>();

                        // Récupération de la lettre de solution
                        char correctLetter = _solutionGrid[x, y];

                        // Initialisation : on passe le Manager lui-même
                        cellScript.Initialize(x, y, correctLetter, this);

                        // Stocker la référence
                        _cellObjects[x, y] = cellScript;
                    }
                    else
                    {
                        Instantiate(emptyCellPrefab, gridContainer);
                    }
                }
            }
        }

        // Nouvelle méthode de vérification appelée par GridCell.cs
        /*public void CheckLetter(GridCell cell, char enteredChar)
        {
            // 1. Mise à jour de la grille du joueur
            _playerGrid[cell.X, cell.Y] = enteredChar;

            // 2. Vérification immédiate
            if (enteredChar == cell.CorrectLetter)
            {
                Debug.Log($"Correct ! ({cell.X},{cell.Y})");
                cell.SetBackground(Color.green);
                cell.SetInputInteractable(false);

                // 3. (À ajouter) Vérifier si le mot est complet et si le niveau est terminé
                // CheckWordCompletion(cell.X, cell.Y);
                // CheckLevelCompletion();
            }
            else
            {
                Debug.Log($"Faux. Attendu : {cell.CorrectLetter}");
                cell.SetBackground(Color.red); //
            }
        }*/


    }
}
