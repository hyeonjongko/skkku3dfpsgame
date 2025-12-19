using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject _explosionEffectPrefab;

    public float ExplosionRadius = 2;
    public float Damage = 1000;

    [Header("넉백")]
    public float ExplosionKnockbackForce = 10f;
    public float ExplosionUpwardForce = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        // 내 위치에 폭탄 이펙트 생성
        GameObject effectObject = Instantiate(_explosionEffectPrefab);
        effectObject.transform.position = transform.position;

        // 목표 : 폭발했을때 일정범위안에 몬스터가 있다면 대미지를 주고싶다.
        // 속성:
        //  - 폭발 반경
        //  - 대미지

        Vector3 position = transform.position;

        // 1. 씬을 모두 순회하면서 게임 오브젝트를 찾는다. 1000 번 순회
        // 2. 모든 몬스터를 순회하면서 거리를 측정한다..   500 번 순회
        /*Monster[] monsters = FindObjectsOfType<Monster>();
        for (int i = 0; i < monsters.Length; i++)
        {
            if(몬스터와의 거리 < ExplosionRadius)
            {
                대미지를 준다.
            }
        }*/

        //가상의 구를 만들어서 그 구 영역에 안에 있는 모든 콜라이더를 찾아서 배열로 반환한다.
        //NameToLayer: 문자열 → 레이어 인덱스(int) : 특정 레이어의 숫자 인덱스가 필요할 때
        //GetMask: 문자열 목록 → 비트 마스크(int) : 레이어 마스크 값이 필요한 함수(Raycast, OverlapSphere 등)에서 여러 레이어를 지정할 때
        Collider[] colliders_Monster = Physics.OverlapSphere(position, ExplosionRadius, LayerMask.GetMask("Monster"));

        Damage damage = new Damage()
        {
            Value = Damage,
            HitPoint = transform.position,
            Who = this.gameObject,
            Critical = false,
        };

        for (int i = 0; i < colliders_Monster.Length; i++)
        {
            Monster monster = colliders_Monster[i].gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                float distance = Vector3.Distance(position, monster.transform.position);
                distance = Mathf.Min(1f, distance);

                float finalDamage = Damage / distance; // 폭발 원점과 거리에 따라서 데미지를 다르게 준다.

                damage.Value = finalDamage;


                // 넉백 방향 계산 (폭발 중심 -> 몬스터 방향 = 피격 방향 반대)
                Vector3 knockbackDirection = (monster.transform.position - position).normalized;
                knockbackDirection += Vector3.up * ExplosionUpwardForce;

                // 거리에 반비례하는 넉백 (가까울수록 강하게)
                float knockbackMultiplier = 1f / distance;

                damage.knockbackDirection = knockbackDirection * ExplosionKnockbackForce * knockbackMultiplier;

                monster.TryTakeDamage(damage);
            }
        }

        Collider[] colliders_Drum = Physics.OverlapSphere(position, ExplosionRadius, LayerMask.GetMask("Drum"));

        for (int i = 0; i < colliders_Drum.Length; i++)
        {
            Drum drum = colliders_Drum[i].gameObject.GetComponent<Drum>();

            if (drum != null)
            {
                float distance = Vector3.Distance(position, drum.transform.position);
                distance = Mathf.Min(1f, distance);

                float finalDamage = Damage / distance; // 폭발 원점과 거리에 따라서 데미지를 다르게 준다.

                damage.Value = finalDamage;

                drum.TryTakeDamage(damage);
            }
        }

        //if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        //{
        //    Monster monster = collision.gameObject.GetComponent<Monster>();
        //    monster.TryTakeDamage(Damage);
        //}

        // 충돌하면 나 자신을 삭제한다.
        gameObject.SetActive(false);
    }
}
