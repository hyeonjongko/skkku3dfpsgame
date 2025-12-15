using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Monster))]
public class MonsteHealthBar : MonoBehaviour
{
    private Monster _monster;

    [SerializeField] private Image _guageImage;
    [SerializeField] private Transform _healthBarTransform;

    private float _lastHealth = 0;

    void Awake()
    {
        _monster = gameObject.GetComponent<Monster>();
    }

    void LateUpdate() // 체력 연산이 끝난 다음에 업데이트가 되도록 LateUpdate() 사용
    {
        if (_lastHealth != _monster.Health.Value)
        {
            _lastHealth = _monster.Health.Value;
            _guageImage.fillAmount = _monster.Health.Value / _monster.Health.MaxValue;
        }

        //빌보드 기법 : 카메라의 위치와 회전에 상관없이 항상 정면을 바라보게 하는 기법
        _healthBarTransform.forward = Camera.main.transform.forward;

    }
}
