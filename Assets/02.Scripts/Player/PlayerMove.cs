using System;
using UnityEngine;
using UnityEngine.AI;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMove : MonoBehaviour
{
    [Serializable] // json, sciptableObject 혹은 DB에서 읽어오게 하면된다.
    public class MoveConfig
    {
        public float Gravity;
        public float RunStamina;
        public float JumpStamina;
    }

    public MoveConfig _config;

    private Animator _animator;


    private CharacterController _controller;
    private PlayerStats _stats;

    [SerializeField] private NavMeshAgent _agent;
    RaycastHit rayHit = new RaycastHit();

    private float _yVelocity = 0f;   // 중력에 의해 누적될 y값 변수

    private int _jumpCount = 2;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _agent.speed = _stats.MoveSpeed.Value;

    }

    private void Update()
    {
        // 0. 중력을 누적한다.
        _yVelocity += _config.Gravity * Time.deltaTime;

        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal"); // -1 ~ 1로  점차 변함
        float y = Input.GetAxis("Vertical");

        // 2. 입력에 따른 방향 구하기 
        // 현재는 유니티 세상의 절대적인 방향이 기준 (글로벌/월드 좌표계)
        // 내가 원하는 것은 카메라가 쳐다보는 방향이 기준으로

        // - 글로벌 좌표 방향을 구한다. 
        Vector3 direction = new Vector3(x, 0, y);
        _animator.SetFloat("Speed", direction.magnitude);

        direction.Normalize();

        if(GameManager.Instance.State == EGameState.Playing)
        {
            // - 점프! : 점프 키를 누르고 && 땅이라면
            if (Input.GetButtonDown("Jump") && _controller.isGrounded && _jumpCount == 2)
            {
                Jump();
            }
            else if (Input.GetButtonDown("Jump") && _jumpCount == 1)
            {
                if (_stats.Stamina.TryConsume(_config.JumpStamina))
                {
                    Jump();
                }
            }
            else if (_controller.isGrounded)
            {
                _jumpCount = 2;
            }

            // - 카메라가 쳐다보는 방향으로 변환한다. (월드 -> 로컬)
            direction = Camera.main.transform.TransformDirection(direction);
            direction.y = _yVelocity; // 중력 적용


            float moveSpeed = _stats.MoveSpeed.Value;
            if (Input.GetKey(KeyCode.LeftShift) && _controller.isGrounded)
            {
                if (_stats.Stamina.TryConsume(_config.RunStamina * Time.deltaTime))
                {
                    moveSpeed = _stats.RunSpeed.Value;
                }
            }

            // 3. 방향으로 이동시키기  
            _controller.Move(direction * moveSpeed * Time.deltaTime);
        }
        if (GameManager.Instance.State == EGameState.TopView)
        {
            // 마우스 왼쪽 클릭 감지
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Raycast로 클릭한 지점 찾기
                if (Physics.Raycast(ray.origin, ray.direction, out rayHit));
                {
                    // NavMeshAgent로 목표 지점 설정
                    _agent.SetDestination(rayHit.point);
                }
            }
        }


    }
    private void Jump()
    {
        _yVelocity = _stats.JumpPower.Value;
        _jumpCount--;
    }

}
