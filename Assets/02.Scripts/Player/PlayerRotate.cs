using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 200f; // 0 ~ 360
    private float _accumulationX = 0;

    public CameraFollow cameraFollow;

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing) return;

        if (cameraFollow != null && cameraFollow.IsTopView) return;

        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, _accumulationX);
    }
}
