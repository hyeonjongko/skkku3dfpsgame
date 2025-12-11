using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    private PlayerGunFire _playerGunFire;
    [SerializeField] private Slider _reloadSlider;
    [SerializeField] private TextMeshProUGUI _bulletCountText;

    void Start()
    {
        // Player 태그로 찾기 (플레이어에 Player 태그가 있어야 함)
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            _playerGunFire = playerObject.GetComponent<PlayerGunFire>();
        }
    }

    void Update()
    {
        if (_playerGunFire != null && _reloadSlider != null)
        {
            _reloadSlider.value = _playerGunFire.ReloadProgress;
        }
        if (_playerGunFire != null && _bulletCountText != null)
        {
            // "현재탄창 / 예비탄약" 형식
            //_bulletCountText.text = $"{_playerGunFire._bulletCount} / {_playerGunFire.ReverseBullets}";
        }
    }
}
