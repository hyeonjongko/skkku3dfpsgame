using UnityEngine;

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
        // Q키 입력은 항상 체크
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _isTopView = !_isTopView;
        }

        // TopView 모드
        if (_isTopView)
        {
            transform.position = TopViewTarget.position;
            transform.rotation = TopViewTarget.rotation;
            return; // TopView일 때는 여기서 끝
        }

        // Playing 상태가 아니면 더 이상 진행하지 않음
        if (GameManager.Instance.State != EGameState.Playing) return;

        // T키로 FPS/TPS 전환
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isFPS = !_isFPS;
        }

        // FPS/TPS 모드
        if (_isFPS)
        {
            transform.position = FPSTarget.position;
            transform.rotation = FPSTarget.rotation; // rotation 추가!
        }
        else
        {
            transform.position = TPSTarget.position;
            transform.rotation = TPSTarget.rotation; // rotation 추가!
        }
    }
}
