using UnityEngine;

// GameManager에 추가


// CursorManager 수정
public class CursorManager : MonoBehaviour
{
    private EGameState _state;
    void Update()
    {
        if(_state == EGameState.Ready)
        {
            LockCursor();
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKey(KeyCode.Tab))
        {
            _state = EGameState.UIMode;
            UnlockCursor();
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Tab))
        {
            _state = EGameState.Playing;
            LockCursor();
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
