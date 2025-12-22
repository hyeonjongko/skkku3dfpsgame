using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateTextUI;

    [SerializeField] private float _readyTime = 2.0f;
    private float _startTextTime = 0.5f;

    private float _gameOverTime = 0.5f;

    [SerializeField] private PlayerStats _stats;

    [SerializeField] private CameraFollow _view;

    private bool isChangingState = false;

    private void Awake()
    {
        _instance = this;
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
