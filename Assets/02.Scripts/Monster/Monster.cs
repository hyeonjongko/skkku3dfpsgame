using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static PlayerMove;

public class Monster : MonoBehaviour, IDamageable
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

    public ConsumableStat Health;

    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    [SerializeField] private PlayerStats _playerStats;

    //private MonsterAttack _monsterAttack;


    public float DetectDistance = 2.0f;
    public float AttackDistance = 1.2f;

    public float MoveSpeed = 5.0f;
    public float AttackSpeed = 2f;
    public float AttackTimer = 0f;
    public float AttackDamage = 20.0f;

    [Header("복귀")]
    private Vector3 _comebackPosition;
    private float _comebackPosoffset = 0.5f;

    [Header("순찰")]
    [SerializeField] private Transform[] _patrolPoints; // 순찰 지점들
    [SerializeField] private float _patrolWaitTime = 2f; // 각 지점에서 대기 시간
    [SerializeField] private float _patrolArriveDistance = 0.5f;
    private int _currentPatrolIndex = 0; // 현재 목표 순찰 지점 인덱스
    private float _patrolWaitTimer = 0f; // 대기 타이머
    private bool _isWaitingAtPatrolPoint = false; // 순찰 지점에서 대기 중인지
    [Header("중력")]
    private float Gravity;
    private float _yVelocity = 0f;


    [Header("넉백")]
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    private Vector3 _knockbackVelocity = Vector3.zero;

    private Vector3 _jumpStartPosition;
    private Vector3 _jumpEndPosition;

    private float _offset = 1f;

    private void Awake()
    {
        if(_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        //_monsterAttack = GetComponentInChildren<MonsterAttack>();
    }
    private void Start()
    {
        _comebackPosition = transform.position;
        _agent.speed = MoveSpeed;
        _agent.stoppingDistance = AttackDistance;
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing) return;

        // 0. 중력을 누적한다.
        _yVelocity += Gravity * Time.deltaTime;

        // 넉백 적용
        if (_knockbackVelocity.magnitude > 0.1f)
        {
            _controller.Move(_knockbackVelocity * Time.deltaTime);
            _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, Time.deltaTime / _knockbackDuration);
        }

        //몬스터의 상태에 따라 다른 행동을 한다 (다른 메서드를 호출한다.)
        switch (State)
        {
            case EMonsterState.Idle:
                Idle();
                break;
            case EMonsterState.Patrol:
                Patrol();
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
            case EMonsterState.Jump:
                Jump();
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
            _animator.SetTrigger("IdleToTrace");
            Debug.Log("상태 전환 : Idle -> Trace");
        }
        _patrolWaitTimer += Time.deltaTime;

        if (_patrolWaitTimer >= _patrolWaitTime)
        {
            State = EMonsterState.Patrol;
            _animator.SetTrigger("IdleToPatrol");
            Debug.Log("상태 전환 : Idle -> Patrol");
        }

    }

    private void Patrol()
    {
        // 순찰 지점이 설정되지 않았다면 Idle 상태로 전환
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            State = EMonsterState.Idle;
            Debug.Log("상태 전환 : Patrol -> Idle (순찰 지점 없음)");
            return;
        }

        // 플레이어 감지 체크
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            _animator.SetTrigger("PatrolToTrace");
            _isWaitingAtPatrolPoint = false;
            _patrolWaitTimer = 0f;
            Debug.Log("상태 전환 : Patrol -> Trace");
            return;
        }

        // 순찰 지점에서 대기 중이라면
        if (_isWaitingAtPatrolPoint)
        {
            _patrolWaitTimer += Time.deltaTime;

            // 대기 시간이 끝나면 다음 지점으로 이동
            if (_patrolWaitTimer >= _patrolWaitTime)
            {
                _isWaitingAtPatrolPoint = false;
                _patrolWaitTimer = 0f;

                // 다음 순찰 지점으로 인덱스 증가 (순환)
                _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            }
            return;
        }

        // 현재 목표 순찰 지점
        Vector3 targetPosition = _patrolPoints[_currentPatrolIndex].position;

        // 1. 방향을 구한다 (Y축 제외한 수평 방향)
        Vector3 directionFlat = targetPosition - transform.position;
        directionFlat.y = 0; // Y축 무시
        directionFlat.Normalize();

        // 2. 이동한다
        _controller.Move(directionFlat * MoveSpeed * Time.deltaTime);

        // 3. 목표 지점에 도착했는지 확인 (수평 거리만 계산)
        Vector3 posFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetFlat = new Vector3(targetPosition.x, 0, targetPosition.z);
        float distanceToTarget = Vector3.Distance(posFlat, targetFlat);


        if (distanceToTarget <= _patrolArriveDistance)
        {
            // 순찰 지점에 도착 - 대기 시작
            _isWaitingAtPatrolPoint = true;
            _patrolWaitTimer = 0f;
        }
    }

    private void Trace()
    {
        //플레이어를 쫓아간다
        //Todo. Run 애니메이션 실행

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        //1. 방향을 구한다.
        //Vector3 direction = (_player.transform.position - transform.position).normalized;
        //_controller.Move(direction * MoveSpeed * Time.deltaTime);

        //방향 설정도 필요 없이 도착지만 설정해주면 네비게이션 시스템에 의해 자동으로 이동한다.
        _agent.SetDestination(_player.transform.position);

        //플레이어와의 거리가 공격 범위보다 가깝다면
        if (distance <= AttackDistance)
        {
            State = EMonsterState.Attack;
            Debug.Log("상태 전환 : Trace -> Attack");
        }

        if (_agent.isOnOffMeshLink)
        {
            Debug.Log("링크 만남");
            OffMeshLinkData LinkData = _agent.currentOffMeshLinkData;
            _jumpStartPosition = LinkData.startPos;
            _jumpEndPosition = LinkData.endPos;

            if (_jumpEndPosition.y > _jumpStartPosition.y)
            {
                Debug.Log("상태 전환 : Trace -> Jump");
                State = EMonsterState.Jump;
                return;
            }
        }
        //if (Vector3.Distance(transform.position, _player.transform.position) > DetectDistance)
        //{
        //    _animator.SetTrigger("TraceToComeback");
        //    State = EMonsterState.Comeback;
        //    Debug.Log("상태 전환 : Trace -> Comeback");
        //}
    }

    private void Jump()
    {
        // 순간이동
        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.CompleteOffMeshLink();

        StartCoroutine(Jump_Coroutine());


        // 1. 점프 거리와 내 이동속를 계산해서 점프 시간을 구한다.
        // 2. 점프 시간동안 포물선으로 이동한다.
        // 3. 이동 후 다시 Trace
    }

    private IEnumerator Jump_Coroutine()
    {
        float distance = Vector3.Distance(_jumpStartPosition, _jumpEndPosition);
        float jumpTime = distance / MoveSpeed;
        float jumpHeight = Mathf.Max(1.5f, distance * 0.5f);  // 높이를 조금 더 높게

        float elapsedTime = 0f;

        while (elapsedTime < jumpTime)
        {
            float t = elapsedTime / jumpTime;

            Vector3 adjustedEndPos = _jumpEndPosition;
            adjustedEndPos.y += _offset;

            // 수평 이동 (Linear)
            Vector3 newPosition = Vector3.Lerp(_jumpStartPosition, _jumpEndPosition, t);

            // 수직 이동 (포물선) - 0에서 시작해서 중간에 최고점, 다시 0으로
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            newPosition.y = Mathf.Lerp(_jumpStartPosition.y, adjustedEndPos.y, t) + height;

            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 finalPosition = _jumpEndPosition;
        finalPosition.y += _offset;
        transform.position = finalPosition;

        _agent.isStopped = false;  // 이동 재개
        State = EMonsterState.Trace;
        Debug.Log("상태 전환 : Jump -> Trace");
    }

    private void Comeback()
    {
        Vector3 direction = (_comebackPosition - transform.position).normalized;
        _controller.Move(direction * MoveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= DetectDistance)
        {
            _animator.SetTrigger("ComebackToTrace");
            State = EMonsterState.Trace;
            Debug.Log("상태 전환 : Comeback -> Trace");
            return;
        }
        float distanceComeback = Vector3.Distance(transform.position, _comebackPosition);

        if (distanceComeback <= _comebackPosoffset)
        {
            _animator.SetTrigger("ComebackToIdle");
            State = EMonsterState.Idle;
            Debug.Log("상태 전환 : Comeback -> Idle");
        }

    }
    private void Attack()
    {

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance > AttackDistance)
        {
            _animator.SetTrigger("AttackToTrace");
            State = EMonsterState.Trace;
            Debug.Log("상태 전환 : Attack -> Trace");
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            _animator.SetTrigger("Attack");
            AttackTimer = 0f;
            //플레이어를 공격하는 상태
            Debug.Log("플레이어 공격");
            //_playerStats.Health.Consume(AttackDamage);
            //_playerStats.TryTakeDamage(AttackDamage);
            //_monsterAttack.PlayerAttack();

        }


    }

    //public float Health = 100;

    public GameObject bloodEffectPrefab;

    public bool TryTakeDamage(Damage damage)
    {
        //데미지를 받으면 데미지를 받은 위치에 혈흔 이펙트를 생성해서 플레이 하고 싶다.
        // 그 이펙트는 "몬스터를 따라다녀야" 한다.
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, _player.transform.position,Quaternion.identity, transform);
        bloodEffect.transform.forward = damage.Normal;

        if (State == EMonsterState.Death || State == EMonsterState.Hit)
        {
            return false;
        }

        Health.Consume(damage.Value);

        _agent.isStopped = true; // 이동 일시정지
        _agent.ResetPath();      // 경로(=목적지) 삭제

        if (damage.knockbackDirection != Vector3.zero)
        {
            _knockbackVelocity = damage.knockbackDirection.normalized * _knockbackForce;
        }

        if (Health.Value > 0)
        {
            _animator.SetTrigger("Hit");
            Debug.Log($"상태 전환: {State} -> Hit");
            State = EMonsterState.Hit;

            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            _animator.SetTrigger("Death");
            Debug.Log($"상태 전환: {State} -> Death");
            State = EMonsterState.Death;

            StartCoroutine(Death_Coroutine());
        }

        return true;
    }

    public IEnumerator Hit_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);
        State = EMonsterState.Idle;
        Debug.Log("몬스터가 데미지를 받았습니다.");
    }
    private IEnumerator Death_Coroutine()
    {
        // Todo. Death 애니메이션 실행
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

}
