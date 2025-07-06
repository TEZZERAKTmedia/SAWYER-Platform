using UnityEngine;
using UnityEngine.EventSystems;

public class CellDropZone : MonoBehaviour, IDropHandler
{
    private int row;
    private int col;
    private DynamicCellGridEditor gridEditor;

    public void Initialize(int r, int c, DynamicCellGridEditor editor)
    {
        row = r;
        col = c;
        gridEditor = editor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            gridEditor.AssignObjectToCell(row, col, droppedObject);
        }
    }
}
