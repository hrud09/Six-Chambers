using UnityEngine;
public class ScreenshotCapture : MonoBehaviour
{
    int count = 0;
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot($"/Users/pixelman360/Documents/Big Bang Projects/SS/Level-{count++}.png");
        }
#endif
    }
}