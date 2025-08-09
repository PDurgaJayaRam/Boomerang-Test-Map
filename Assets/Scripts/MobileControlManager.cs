using UnityEngine;

public class MobileControlManager : MonoBehaviour
{
    public GameObject mobileControlsCanvas;
    
    void Start()
    {
        // Show controls on mobile, hide on other platforms
        #if UNITY_ANDROID || UNITY_IOS
        mobileControlsCanvas.SetActive(true);
        #else
        mobileControlsCanvas.SetActive(false);
        #endif
    }
}