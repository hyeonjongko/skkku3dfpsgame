using UnityEngine;

// 목표를 따라다니는 카메라
public class CameraFollow : MonoBehaviour
{
    public Transform FPSTarget;
    public Transform TPSTarget;
    public Transform TopViewTarget;

    private bool _isFPS = true;

    public bool IsFPS => _isFPS;

    private bool _isTopView = false;
    public bool IsTopView => _isTopView;

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
            
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _isTopView = !_isTopView;
        }
        if (_isTopView)
        {
            transform.position = TopViewTarget.position;
            transform.rotation = TopViewTarget.rotation;
        }
        else
        {
            if (_isFPS)
            {
                transform.position = FPSTarget.position;

            }
            else
            {
                transform.position = TPSTarget.position;

            }

        }
    }

}
