using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;  // Assign the Fill image in Inspector
    private Transform target; // Who the bar follows
    private Vector3 offset;   // Position offset above head

    public void Initialize(Transform followTarget, Vector3 positionOffset)
    {
        target = followTarget;
        offset = positionOffset;
    }

    public void UpdateHealth(float normalizedValue)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
