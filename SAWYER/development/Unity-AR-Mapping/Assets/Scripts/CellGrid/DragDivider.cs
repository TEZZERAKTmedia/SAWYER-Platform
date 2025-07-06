using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDivider : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private DynamicCellGridEditor gridEditor;
    private bool isHorizontal;
    private int dividerIndex;
    private Vector2 lastMousePosition;
    private bool isDraggingGlobal;
    private RectTransform rectTransform;

    public void Initialize(DynamicCellGridEditor editor, bool horizontal, int index)
    {
        gridEditor = editor;
        isHorizontal = horizontal;
        dividerIndex = index;
        rectTransform = GetComponent<RectTransform>();

        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.color = new Color(0.5f, 0.5f, 1f, 0.8f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;

        RectTransform gridRect = gridEditor.gridContainer;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, eventData.position, eventData.pressEventCamera, out localPoint);

        Rect gridBounds = gridRect.rect;
        isDraggingGlobal = !gridBounds.Contains(localPoint);

        GetComponent<Image>().color = isDraggingGlobal ? Color.red : Color.yellow;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastMousePosition;
        Vector2 localDelta = delta / gridEditor.canvas.scaleFactor;

        gridEditor.OnDividerDrag(isHorizontal, dividerIndex, localDelta, isDraggingGlobal);
        lastMousePosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().color = gridEditor.gridLineColor;
    }
}
