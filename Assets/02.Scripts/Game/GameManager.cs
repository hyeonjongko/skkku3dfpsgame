using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateTextUI;
    [SerializeField] private UI_OptionPopup _optionUI;
    [SerializeField] private float _readyTime = 2.0f;
    private float _startTextTime = 0.5f;
    private float _gameOverTime = 0.5f;
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private CameraFollow _view;

    private bool isChangingState = false;
    private bool _isOptionOpen = false;
    public bool IsOptionOpen => _isOptionOpen;
    private CursorManager _cursorManager;

    private void Awake()
    {
        _instance = this;
        _cursorManager = FindObjectOfType<CursorManager>();
    }

    private void Start()
    {
        _state = EGameState.Ready;
        _stateTextUI.text = "준비중...";

        StartCoroutine(StartToPlay_Coroutuine());
    }

    private void Update()
    {
        if (_stats.Health.Value < 0)
        {
            StartCoroutine(GameOver_Coroutuine());
        }

        if (!isChangingState)
        {
            if (_view.IsTopView && _state != EGameState.TopView)
            {
                StartCoroutine(TopView_Coroutuine());
            }
            else if (!_view.IsTopView && _state == EGameState.TopView)
            {
                StartCoroutine(Playing_Coroutuine());
            }
        }

        // ESC 키로 옵션 창 열기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            _isOptionOpen = true;
            if (_cursorManager != null)
            {
                _cursorManager.UnlockCursor();
            }
            _optionUI.Show();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Time.timeScale = 1;
        _isOptionOpen = false;
        // 옵션 창을 닫을 때 커서 다시 잠금
        if (_cursorManager != null && _state == EGameState.Playing)
        {
            _cursorManager.LockCursor();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LoadingScene");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator StartToPlay_Coroutuine()
    {
        yield return new WaitForSeconds(_readyTime);
        _stateTextUI.text = "시작";
        yield return new WaitForSeconds(_startTextTime);
        _state = EGameState.Playing;
        _stateTextUI.gameObject.SetActive(false);
    }

    private IEnumerator GameOver_Coroutuine()
    {
        _stateTextUI.gameObject.SetActive(true);
        _stateTextUI.text = "게임 오버";
        _state = EGameState.GameOver;
        yield return new WaitForSeconds(_gameOverTime);
        _stateTextUI.gameObject.SetActive(false);
    }

    private IEnumerator TopView_Coroutuine()
    {
        isChangingState = true;
        Debug.Log("탑뷰");
        yield return new WaitForSeconds(0.2f);
        _state = EGameState.TopView;
        isChangingState = false;
    }

    private IEnumerator Playing_Coroutuine()
    {
        isChangingState = true;
        Debug.Log("FPS & TPS 뷰");
        yield return new WaitForSeconds(0.2f);
        _state = EGameState.Playing;
        isChangingState = false;
    }
}
