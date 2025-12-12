using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Monster : MonoBehaviour
{
    #region Comment
    // 목표 : 처음엔 가만히 있지만 플레이어가 다가가면 쫓아오는 좀비 몬스터를 만들고 싶다.
    //   ㄴ>   쫓아 오다가 너무 멀어지면 제자리로 돌아간다.

    // Idle : 가만히 있는다.
    //   I   (플레이어가 가까이 오면) (컨디션, 트렌지션이라고 한다.)
    // Trace : 플레이어를 쫓아간다
    //   I   (플레이어와 너무 멀어지면)
    // Return : 제자리로 돌아가는 상태
    //   I   (제자리에 도착했다면)
    // Idle
    //공격
    //피격
    //죽음

    // 몬스터 인공지는(AI) : 사람처럼 행동하는 똑똑한 시스템 / 알고리즘
    // - 규칙 기반 인공지능 : 정해진 규칙에 따라 조건문/반복문등을 이용해서 코딩하는 것
    //                        ->FSM(유한 상태머신), BT(행동 트리)
    // - 학습 기반 인공지능 : 머신러닝(딥러닝, 강화학습..)

    //Finite State Machine(유한 상태 머신)
    //N개의 상태를 가지고 있고, 상태마다 행동이 다르다.
    #endregion

    public EMonsterState State = EMonsterState.Idle;

    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerStats _stats;

    public float DetectDistance = 2.0f;
    public float AttackDistance = 1.2f;
    
    public float MoveSpeed = 5.0f;
    public float AttackSpeed = 2f;
    public float AttackTimer = 0f;
    public float AttackDamage = 20.0f;

    [Header("복귀")]
    private Vector3 _comebackPosition;
    private float _comebackPosoffset = 0.5f;

    private void Start()
    {
        _comebackPosition = transform.position;
    }

    private void Update()
    {
        //몬스터의 상태에 따라 다른 행동을 한다 (다른 메서드를 호출한다.)
        switch (State)
        {
            case EMonsterState.Idle:
                Idle();
                break;
            case EMonsterState.Trace:
                Trace();
                break;
            case EMonsterState.Comeback:
                Comeback();
                break;
            case EMonsterState.Attack:
                Attack();
                break;

        }

    }

    //1. 함수는 한 가지 일만 잘해야 한다.
    //2. 상태별 행동을 함수로 만든다.
    private void Idle()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환 : Idle -> Trace");
        }
    }
    private void Trace()
    {
        //플레이어를 쫓아간다
        //Todo. Run 애니메이션 실행

        //1. 방향을 구한다.
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _controller.Move(direction * MoveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        //플레이어와의 거리가 공격 범위보다 가깝다면
        if (distance <= AttackDistance)
        {
            State = EMonsterState.Attack;
            Debug.Log("상태 전환 : Trace -> Attack");
        }
        if (Vector3.Distance(transform.position, _player.transform.position) > DetectDistance)
        {
            State = EMonsterState.Comeback;
            Debug.Log("상태 전환 : Trace -> Comeback");
        }
    }
    private void Comeback()
    {
        Vector3 direction = (_comebackPosition - transform.position).normalized;
        _controller.Move(direction * MoveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환 : Comeback -> Trace");
            return;
        }
        float distanceComeback = Vector3.Distance(transform.position, _comebackPosition);
        
        if(distanceComeback <= _comebackPosoffset)
        {
            State = EMonsterState.Idle;
            Debug.Log("상태 전환 : Comeback -> Idle");
        }

    }
    private void Attack()
    {

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance > AttackDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환 : Attack -> Trace");
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            AttackTimer = 0f;
            //플레이어를 공격하는 상태
            Debug.Log("플레이어 공격");
            _stats.Health.Consume(AttackDamage);

        }
        

    }

    public float Health = 100;
    public bool TryTakeDamage(float damage)
    {
        if (State == EMonsterState.Death || State == EMonsterState.Hit)
        {
            return false;
        }
        Health -= damage;

        if(Health > 0)
        {
            State = EMonsterState.Hit;
        }
        else
        {
            State = EMonsterState.Death;
        }

        return true;
    }
    private IEnumerator Hit_Coroutine()
    {
        // Todo. Hit 애니메이션 실행
        yield return new WaitForSeconds(0.2f);
        State = EMonsterState.Idle;
    }
    private IEnumerator Death_Coroutine()
    {
        // Todo. Death 애니메이션 실행
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

}
