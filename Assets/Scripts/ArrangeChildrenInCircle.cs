using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ArrangeChildrenInCircle : MonoBehaviour
{
    public float radius = 5f;  // Radius of the circle
    public float fromAngle = 0f;  // Starting angle in degrees (0 to 360)
    public float toAngle = 360f;  // Ending angle in degrees (0 to 360)

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ArrangeChildren();
        }
    }


    void ArrangeChildren()
    {
        int childCount = transform.childCount;
        float angleStep = (toAngle - fromAngle) / childCount;

        for (int i = 0; i < childCount; i++)
        {
            float angle = fromAngle + i * angleStep;
            Vector3 positionOffset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
            transform.GetChild(i).position = transform.position + positionOffset;
            transform.GetChild(i).forward = transform.GetChild(i).position - transform.position;
        }
    }
}
