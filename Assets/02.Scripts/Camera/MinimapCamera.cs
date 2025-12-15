using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _offsetY = 10f;

    public Camera targetCamera;
    public float zoomStep = 1f;
    public float minSize = 1f;
    public float maxSize = 10f;
    public float smoothSpeed = 5f;


    public void ZoomIn()
    {
        if (targetCamera.orthographic)
        {
            targetCamera.orthographicSize = Mathf.Max(minSize, targetCamera.orthographicSize - zoomStep);
        }
    }

    // 줌 아웃 (Size 증가)
    public void ZoomOut()
    {
        if (targetCamera.orthographic)
        {
            targetCamera.orthographicSize = Mathf.Min(maxSize, targetCamera.orthographicSize + zoomStep);
        }
    }

    // 특정 값으로 설정
    public void SetSize(float newSize)
    {
        if (targetCamera.orthographic)
        {
            targetCamera.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
        }
    }

    void LateUpdate()
    {
        Vector3 targetPosition = _target.position;
        Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);

        transform.position = finalPosition;


        Vector3 targetAngle = _target.eulerAngles;
        targetAngle.x = 90f;

        transform.eulerAngles = targetAngle;


    }
}
