using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DynamicCellGridEditor : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 3;
    public int columns = 3;
    public float minCellSize = 50f;
    public Color gridLineColor = Color.gray;
    public float gridLineWidth = 2f;

    [Header("UI References")]
    public Canvas canvas;
    public RectTransform gridContainer;
    public GameObject cellPrefab;
    public GameObject dividerPrefab;

    [Header("Grid Data")]
    public List<CellData> cells = new List<CellData>();

    private List<List<RectTransform>> cellGrid;
    private List<DragDivider> horizontalDividers;
    private List<DragDivider> verticalDividers;
    private List<float> rowHeights;
    private List<float> columnWidths;

    void Start()
    {
        InitializeGrid();
        CreateGrid();
    }

    void InitializeGrid()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (gridContainer == null) gridContainer = GetComponent<RectTransform>();

        cellGrid = new List<List<RectTransform>>();
        horizontalDividers = new List<DragDivider>();
        verticalDividers = new List<DragDivider>();
        rowHeights = new List<float>();
        columnWidths = new List<float>();

        float cellHeight = gridContainer.rect.height / rows;
        float cellWidth = gridContainer.rect.width / columns;

        for (int i = 0; i < rows; i++)
            rowHeights.Add(cellHeight);

        for (int i = 0; i < columns; i++)
            columnWidths.Add(cellWidth);
    }

    void CreateGrid()
    {
        ClearGrid();

        for (int row = 0; row < rows; row++)
        {
            List<RectTransform> rowCells = new List<RectTransform>();

            for (int col = 0; col < columns; col++)
            {
                GameObject cellObj = CreateCell(row, col);
                RectTransform cellRect = cellObj.GetComponent<RectTransform>();
                rowCells.Add(cellRect);

                CellData cellData = new CellData(row, col);
                cellData.cellRect = cellRect;
                cells.Add(cellData);
            }

            cellGrid.Add(rowCells);
        }

        CreateDividers();
        UpdateGridLayout();
    }

    GameObject CreateCell(int row, int col)
    {
        GameObject cell;

        if (cellPrefab != null)
        {
            cell = Instantiate(cellPrefab, gridContainer);
        }
        else
        {
            cell = new GameObject($"Cell_{row}_{col}");
            cell.transform.SetParent(gridContainer);

            Image img = cell.AddComponent<Image>();
            img.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);

            Outline outline = cell.AddComponent<Outline>();
            outline.effectColor = gridLineColor;
            outline.effectDistance = Vector2.one;
        }

        CellDropZone dropZone = cell.AddComponent<CellDropZone>();
        dropZone.Initialize(row, col, this);

        return cell;
    }

    void CreateDividers()
    {
        for (int i = 0; i < rows - 1; i++)
        {
            GameObject divider = CreateDivider(true, i);
            DragDivider dragDivider = divider.GetComponent<DragDivider>();
            dragDivider.Initialize(this, true, i);
            horizontalDividers.Add(dragDivider);
        }

        for (int i = 0; i < columns - 1; i++)
        {
            GameObject divider = CreateDivider(false, i);
            DragDivider dragDivider = divider.GetComponent<DragDivider>();
            dragDivider.Initialize(this, false, i);
            verticalDividers.Add(dragDivider);
        }
    }

    GameObject CreateDivider(bool isHorizontal, int index)
    {
        GameObject divider;

        if (dividerPrefab != null)
        {
            divider = Instantiate(dividerPrefab, gridContainer);
        }
        else
        {
            divider = new GameObject($"{(isHorizontal ? "H" : "V")}Divider_{index}");
            divider.transform.SetParent(gridContainer);

            Image img = divider.AddComponent<Image>();
            img.color = gridLineColor;

            divider.AddComponent<DragDivider>();
        }

        return divider;
    }

    void UpdateGridLayout()
    {
        float totalHeight = gridContainer.rect.height;
        float totalWidth = gridContainer.rect.width;

        float currentY = totalHeight / 2;

        for (int row = 0; row < rows; row++)
        {
            float currentX = -totalWidth / 2;
            float rowHeight = rowHeights[row];

            for (int col = 0; col < columns; col++)
            {
                float colWidth = columnWidths[col];
                RectTransform cellRect = cellGrid[row][col];

                cellRect.anchoredPosition = new Vector2(currentX + colWidth / 2, currentY - rowHeight / 2);
                cellRect.sizeDelta = new Vector2(colWidth - gridLineWidth, rowHeight - gridLineWidth);

                currentX += colWidth;
            }

            currentY -= rowHeight;
        }

        UpdateDividerPositions();
    }

    void UpdateDividerPositions()
    {
        float totalHeight = gridContainer.rect.height;
        float totalWidth = gridContainer.rect.width;

        float currentY = totalHeight / 2;
        for (int i = 0; i < horizontalDividers.Count; i++)
        {
            currentY -= rowHeights[i];
            RectTransform dividerRect = horizontalDividers[i].GetComponent<RectTransform>();
            dividerRect.anchoredPosition = new Vector2(0, currentY);
            dividerRect.sizeDelta = new Vector2(totalWidth, gridLineWidth);
        }

        float currentX = -totalWidth / 2;
        for (int i = 0; i < verticalDividers.Count; i++)
        {
            currentX += columnWidths[i];
            RectTransform dividerRect = verticalDividers[i].GetComponent<RectTransform>();
            dividerRect.anchoredPosition = new Vector2(currentX, 0);
            dividerRect.sizeDelta = new Vector2(gridLineWidth, totalHeight);
        }
    }

    public void OnDividerDrag(bool isHorizontal, int index, Vector2 delta, bool isGlobal)
    {
        if (isHorizontal)
        {
            if (isGlobal)
            {
                float newHeight = Mathf.Max(minCellSize, rowHeights[index] - delta.y);
                float heightDiff = newHeight - rowHeights[index];
                rowHeights[index] = newHeight;

                if (index + 1 < rowHeights.Count)
                    rowHeights[index + 1] = Mathf.Max(minCellSize, rowHeights[index + 1] + heightDiff);
            }
            else
            {
                float newHeight = Mathf.Max(minCellSize, rowHeights[index] - delta.y);
                rowHeights[index] = newHeight;

                if (index + 1 < rowHeights.Count)
                    rowHeights[index + 1] = Mathf.Max(minCellSize, rowHeights[index + 1] + delta.y);
            }
        }
        else
        {
            if (isGlobal)
            {
                float newWidth = Mathf.Max(minCellSize, columnWidths[index] + delta.x);
                float widthDiff = newWidth - columnWidths[index];
                columnWidths[index] = newWidth;

                if (index + 1 < columnWidths.Count)
                    columnWidths[index + 1] = Mathf.Max(minCellSize, columnWidths[index + 1] - widthDiff);
            }
            else
            {
                float newWidth = Mathf.Max(minCellSize, columnWidths[index] + delta.x);
                columnWidths[index] = newWidth;

                if (index + 1 < columnWidths.Count)
                    columnWidths[index + 1] = Mathf.Max(minCellSize, columnWidths[index + 1] - delta.x);
            }
        }

        UpdateGridLayout();
    }

    public void AssignObjectToCell(int row, int col, GameObject obj)
    {
        CellData cellData = cells.Find(c => c.row == row && c.col == col);
        if (cellData != null)
        {
            cellData.assignedObject = obj;

            if (obj != null)
            {
                obj.transform.SetParent(cellData.cellRect);
                RectTransform objRect = obj.GetComponent<RectTransform>();
                if (objRect != null)
                {
                    objRect.anchoredPosition = Vector2.zero;
                    objRect.sizeDelta = cellData.cellRect.sizeDelta;
                }
            }
        }
    }

    public void AddRow()
    {
        rows++;
        rowHeights.Add(100f);
        CreateGrid();
    }

    public void AddColumn()
    {
        columns++;
        columnWidths.Add(100f);
        CreateGrid();
    }

    public void RemoveRow()
    {
        if (rows > 1)
        {
            rows--;
            if (rowHeights.Count > 0)
                rowHeights.RemoveAt(rowHeights.Count - 1);
            CreateGrid();
        }
    }

    public void RemoveColumn()
    {
        if (columns > 1)
        {
            columns--;
            if (columnWidths.Count > 0)
                columnWidths.RemoveAt(columnWidths.Count - 1);
            CreateGrid();
        }
    }

    void ClearGrid()
    {
        foreach (Transform child in gridContainer)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        cellGrid.Clear();
        horizontalDividers.Clear();
        verticalDividers.Clear();
        cells.Clear();
    }

    void OnValidate()
    {
        if (Application.isPlaying) return;

        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
        minCellSize = Mathf.Max(10f, minCellSize);
    }
}
