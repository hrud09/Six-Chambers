#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class UIFaceCamera : MonoBehaviour
{
    public bool shouldFaceCamera = false;

    void Update()
    {
        if (!shouldFaceCamera || !Application.isEditor || Application.isPlaying)
            return;

        Camera cam = Camera.main;
        if (cam != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }

        shouldFaceCamera = false; // Reset after execution
    }
}
#endif
