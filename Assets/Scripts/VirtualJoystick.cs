using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Components")]
    public RectTransform background;
    public RectTransform handle;
    public JoystickVisualFeedback visualFeedback;

    [Header("Settings")]
    public float handleRange = 1f;
    public float deadZone = 0.25f;

    private Vector2 inputVector = Vector2.zero;
    public Vector2 InputVector { get { return inputVector; } }

    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            uiCamera = canvas.worldCamera;
        
        // Ensure background is hidden initially
        if (background != null)
            background.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        
        if (visualFeedback != null)
            visualFeedback.SetPressed(true);
            
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, 
            eventData.position, 
            uiCamera, 
            out position
        );

        Vector2 sizeDelta = background.sizeDelta;
        position = new Vector2(
            (position.x / sizeDelta.x) * 2 - 1,
            (position.y / sizeDelta.y) * 2 - 1
        );

        inputVector = position.magnitude > 1 ? position.normalized : position;

        // Apply dead zone
        if (inputVector.magnitude < deadZone)
            inputVector = Vector2.zero;

        handle.anchoredPosition = inputVector * handleRange * (background.sizeDelta / 2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
        
        if (visualFeedback != null)
            visualFeedback.SetPressed(false);
    }
}