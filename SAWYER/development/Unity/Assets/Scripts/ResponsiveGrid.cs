using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGrid : MonoBehaviour
{
    public int columns = 3;
    public int rows = 2;
    public Vector2 spacing = new Vector2(10, 10);
    public bool makeSquare = true;

    [Header("Optional split for 2 cells")]
    [Range(10, 90)]
    public float splitPercentage = 50f;

    private RectTransform rectTransform;
    private GridLayoutGroup grid;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();

        grid.spacing = spacing;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }

    void Start()
    {
        UpdateCellSize();
    }

    void UpdateCellSize()
    {
        if (rectTransform == null)
        rectTransform = GetComponent<RectTransform>();

        if (grid == null)
        grid = GetComponent<GridLayoutGroup>();
        float totalWidth = rectTransform.rect.width;
        float totalHeight = rectTransform.rect.height;

        float cellWidth, cellHeight;

        if (columns == 2 && rows == 1)
        {
            // 2 columns, 1 row
            float spacingTotal = spacing.x;
            cellWidth = (totalWidth - spacingTotal) * (splitPercentage / 100f);
            float otherWidth = (totalWidth - spacingTotal) - cellWidth;
            grid.cellSize = new Vector2(cellWidth, totalHeight);
            Debug.Log($"2 columns: Split {splitPercentage}% → Cell1: {cellWidth}px, Cell2: {otherWidth}px");
        }
        else if (rows == 2 && columns == 1)
        {
            // 2 rows, 1 column
            float spacingTotal = spacing.y;
            cellHeight = (totalHeight - spacingTotal) * (splitPercentage / 100f);
            float otherHeight = (totalHeight - spacingTotal) - cellHeight;
            grid.cellSize = new Vector2(totalWidth, cellHeight);
            Debug.Log($"2 rows: Split {splitPercentage}% → Cell1: {cellHeight}px, Cell2: {otherHeight}px");
        }
        else
        {
            // Regular grid calculation
            cellWidth = (totalWidth - spacing.x * (columns - 1)) / columns;
            cellHeight = (totalHeight - spacing.y * (rows - 1)) / rows;

            if (makeSquare)
            {
                float size = Mathf.Min(cellWidth, cellHeight);
                grid.cellSize = new Vector2(size, size);
            }
            else
            {
                grid.cellSize = new Vector2(cellWidth, cellHeight);
            }
        }
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateCellSize();
    }
}
