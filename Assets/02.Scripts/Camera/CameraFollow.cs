using UnityEngine;

// 목표를 따라다니는 카메라
public class CameraFollow : MonoBehaviour
{
    public Transform FPSTarget;
    public Transform TPSTarget;

    private bool _isFPS = true;
    public bool IsFPS => _isFPS;

    private void Start()
    {
        transform.position = FPSTarget.position;
        transform.rotation = FPSTarget.rotation;
    }
    private void LateUpdate()
    {

        transform.position = FPSTarget.position;

        if (GameManager.Instance.State != EGameState.Playing) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            _isFPS = !_isFPS;

        }
        if (_isFPS)
        {
            transform.position = FPSTarget.position;

        }
        else
        {
            transform.position = TPSTarget.position;
            //transform.rotation = TPSTarget.rotation;
        }
    }

}
