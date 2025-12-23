using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private CameraFollow _cameraFollow;
    private EGameState _state;

    private void Awake()
    {
        _cameraFollow = FindObjectOfType<CameraFollow>();
    }

    void Update()
    {
        // GameManager의 상태 가져오기
        if (GameManager.Instance != null)
        {
            _state = GameManager.Instance.State;
        }

        // 옵션 창이 열려있으면 커서를 잠그지 않음
        if (GameManager.Instance != null && GameManager.Instance.IsOptionOpen)
        {
            return;
        }

        // Ready 상태일 때 커서 잠금
        if (_state == EGameState.Ready)
        {
            LockCursor();
        }

        // Playing 상태일 때 커서 잠금
        if (_state == EGameState.Playing)
        {
            LockCursor();
            if (Input.GetKey(KeyCode.Tab))
            {
                UnlockCursor();
            }
        }

        // TopView 모드일 때 커서 해제
        if (_cameraFollow != null && _cameraFollow.IsTopView)
        {
            UnlockCursor();
            return;
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
