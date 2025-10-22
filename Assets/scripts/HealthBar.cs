using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;

    [Header("Follow Settings")]
    [Tooltip("If true, this bar will track a world position and place itself in screen space (use for Screen Space - Overlay canvases). If false, it stays in world space (use for World Space canvases).")]
    public bool useScreenSpaceFollow = false;

    private Transform followTarget;
    private Vector3 followOffset;
    private Camera cam;
    [SerializeField] private bool billboardToCamera = true;


    void Awake()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>(true);

        cam = Camera.main;
    }

    /// <summary>
    /// Call once after instantiating/placing the bar.
    /// </summary>
    public void Initialize(Transform target, Vector3 offset)
    {
        followTarget = target;
        followOffset = offset;
        UpdateFollowPosition(true);
    }

    /// <summary>
    /// normalized: 0..1
    /// </summary>
    public void UpdateHealth(float normalized)
    {
        if (slider != null)
            slider.value = Mathf.Clamp01(normalized);
    }

  void LateUpdate()
{
    UpdateFollowPosition(false);
    if (billboardToCamera && !useScreenSpaceFollow && Camera.main != null)
    {
       transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        transform.LookAt(Camera.main.transform);
    }
}


    private void UpdateFollowPosition(bool force)
    {
        if (followTarget == null) return;

        if (useScreenSpaceFollow)
        {
            if (cam == null) cam = Camera.main;
            if (cam == null) return;

            Vector3 screen = cam.WorldToScreenPoint(followTarget.position + followOffset);
            transform.position = screen;
        }
        else
        {
            // For a world-space canvas or a bar that’s a child of the target
            transform.position = followTarget.position + followOffset;
        }
    }
}
