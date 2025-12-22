using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //도메인 : 특정 분야의 지식
    public ConsumableStat Health;
    public ConsumableStat Stamina;
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;
    public ValueStat JumpPower;

    public Image BloodEffect;

    [SerializeField] private Animator _animator;
    private void Start()
    {
        Health.Initialize();
        Stamina.Initialize();
        _animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (Health.Value < 0) return;

        float dealtaTime = Time.deltaTime;

        Health.Regenerate(dealtaTime);
        Stamina.Regenerate(dealtaTime);
    }


    public bool TryTakeDamage(float damage)
    {
        if (Health.Value < 0) return false;

        Health.Consume(damage);


        StartCoroutine(BloodEffect_Coroutine());

        //플레이어 레이어 가중치 적용
        _animator.SetLayerWeight(2, Health.Value / Health.MaxValue);

        return true;
    }
    private IEnumerator BloodEffect_Coroutine()
    {
        // 1. 시작 색상 설정 (붉은색, 투명도 0.3)
        Color startColor = new Color(1, 0, 0, 0.5f);
        BloodEffect.color = startColor;

        // 2. 사라지는 데 걸리는 시간 (초 단위) - 이 값을 조절해서 속도 변경 가능
        float duration = 0.5f;
        float elapsedTime = 0f;

        // 3. 시간이 흐름에 따라 서서히 투명하게 만들기
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // startColor에서 Color.clear(투명)로 시간 비율만큼 서서히 변경
            BloodEffect.color = Color.Lerp(startColor, Color.clear, elapsedTime / duration);

            yield return null; // 한 프레임 대기
        }

        // 4. 확실하게 투명 상태로 마무리
        BloodEffect.color = Color.clear;
    }
}
