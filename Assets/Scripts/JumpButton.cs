using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Visual Settings")]
    public Color normalColor = new Color(1, 1, 1, 0.7f);
    public Color pressedColor = new Color(1, 0.2f, 0.2f, 0.9f);
    public Color highlightColor = new Color(1, 1, 1, 0.9f);
    
    [Header("Components")]
    public Image buttonImage;
    public Text buttonText;

    private bool isPressed = false;
    public bool IsPressed { get { return isPressed; } }

    void Start()
    {
        // Show controls in editor for testing
        // Comment this out for final build if you want to hide on non-mobile
        // #if !UNITY_ANDROID && !UNITY_IOS
        // gameObject.SetActive(false);
        // #endif
        
        // Set initial visual state
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        UpdateVisualState(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        UpdateVisualState(false);
    }

    private void UpdateVisualState(bool pressed)
    {
        if (buttonImage != null)
        {
            buttonImage.color = pressed ? pressedColor : normalColor;
        }
        
        // Add a slight scale effect when pressed
        if (transform != null)
        {
            transform.localScale = pressed ? new Vector3(0.95f, 0.95f, 1f) : Vector3.one;
        }
    }
}