using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("References")]
    public RectTransform joystickBase;
    public RectTransform joystickThumb;

    [Header("Settings")]
    public float maxRadius = 100f;

    [Header("Output")]
    public Vector2 inputVector = Vector2.zero;

    private Vector2 startAnchoredPos;

    void Start()
    {
        if (joystickThumb != null)
            startAnchoredPos = joystickThumb.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out pos);

        pos = Vector2.ClampMagnitude(pos, maxRadius);
        joystickThumb.anchoredPosition = pos;

        inputVector = pos / maxRadius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickThumb.anchoredPosition = startAnchoredPos;
        inputVector = Vector2.zero;
    }
}
