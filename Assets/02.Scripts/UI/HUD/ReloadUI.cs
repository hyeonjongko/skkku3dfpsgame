using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    private PlayerGunFire _playerGunFire;
    [SerializeField] private Slider _reloadSlider;

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

    }
}
