using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(-200)]
public class DLJCameraResolution : MonoBehaviour
{
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);
    [SerializeField, Min(0.01f)] private float referenceOrthographicSize = 5f;

    private Camera targetCamera;

    private void OnEnable() => FitResolution();
    private void LateUpdate() => FitResolution();
    private void OnValidate() => FitResolution();

    private void FitResolution()
    {
        if (targetCamera == null) targetCamera = GetComponent<Camera>();
        if (targetCamera == null || !targetCamera.orthographic || targetCamera.aspect <= 0f ||
            referenceResolution.x <= 0f || referenceResolution.y <= 0f) return;

        // CanvasScaler의 Expand와 같은 비율로 월드와 UI를 함께 맞춘다.
        float referenceAspect = referenceResolution.x / referenceResolution.y;
        targetCamera.orthographicSize = referenceOrthographicSize *
            Mathf.Max(1f, referenceAspect / targetCamera.aspect);
    }

    private void OnDisable()
    {
        if (targetCamera != null && targetCamera.orthographic)
            targetCamera.orthographicSize = referenceOrthographicSize;
    }
}
