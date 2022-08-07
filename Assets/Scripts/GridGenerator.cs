using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector2Int cellSize;
    public Vector2Int spacing;
    public GameController gameController;
    public int padding;
    public GameObject gridPrefab;

    public GridLayoutGroup gridLayout;

    [ContextMenu("CreateGrid")]
    public void CreateGrid()
    {
        gridLayout.padding = new RectOffset(padding, padding, padding, padding);
        gridLayout.cellSize = cellSize;
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize.x;
        gameController.gridArray = new Cell[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                gameController.gridArray[x, y] = Instantiate(gridPrefab, transform).GetComponent<Cell>();
                gameController.gridArray[x, y].gameObject.name = $"{x}_{y}";
                gameController.gridArray[x, y].index = new Vector2Int(x, y);
            }
        }
    }
}
