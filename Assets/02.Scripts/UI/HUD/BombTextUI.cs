using TMPro;
using UnityEngine;

public class BombTextUI : MonoBehaviour
{
    private PlayerBombFire _playerBombFire;
    [SerializeField] private TextMeshProUGUI _bombCountText;

    void Start()
    {
        // Player 태그로 찾기 (플레이어에 Player 태그가 있어야 함)
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            _playerBombFire = playerObject.GetComponent<PlayerBombFire>();
        }
    }

    void Update()
    {
        if (_playerBombFire != null && _bombCountText != null)
        {
            _bombCountText.text = _playerBombFire._bombCount.ToString();            
        }
    }
}