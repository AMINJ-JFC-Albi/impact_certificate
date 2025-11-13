using System;
using UnityEngine;

namespace Grid
{
    // Envoie un event avec les coordonnées de la cellule cliquée (x, y)
    public class InputManager : MonoBehaviour
    {
        public static event Action<int, int> OnCellClicked;

        [Tooltip("Position world (0,0) of the grid")]
        public Vector2 gridOrigin = Vector2.zero;
        [Tooltip("Cell siez in word unit")]
        public float cellSize = 1.0f;
        [Tooltip("Size X (column)")]
        public int gridSizeX = 20;
        [Tooltip("Size Y (line)")]
        public int gridSizeY = 20;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 screenPos = Input.mousePosition;

                // Si vous utilisez une grille en UI (Canvas), utilisez RectTransformUtility.ScreenPointToLocalPointInRectangle
                // Ici on suppose une grille en world space orthographique ou sprites alignés.
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                TryConvertWorldPositionToCell(worldPos);
            }

            // Additional feature : arrows keys to navigate the grid
        }

        void TryConvertWorldPositionToCell(Vector3 worldPos)
        {
            // Projeté sur XY
            float localX = worldPos.x - gridOrigin.x;
            float localY = worldPos.y - gridOrigin.y;

            int ix = Mathf.FloorToInt(localX / cellSize);
            int iy = Mathf.FloorToInt(localY / cellSize);

            if (ix >= 0 && ix < gridSizeX && iy >= 0 && iy < gridSizeY)
            {
                OnCellClicked?.Invoke(ix, iy);
            }
        }

        // Variante : if collider, do Raycast2D and look for a Cell component
    }
}