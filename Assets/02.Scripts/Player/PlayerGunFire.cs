
using System.Collections;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    //목표 : 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총알을 날리고 싶다)
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private CameraRotate _cameraRotate;

    [Header("연사 속도")]
    private float _time = 0f;
    public float _delay = 3.5f;

    [Header("장전")]
    [SerializeField] private int _bulletCount = 0;
    public int BulletCount => _bulletCount;
    private int _bulletCountMax = 30;
    public float ReloadTime = 1.6f;
    private int _shootBullets = 0;
    public int ReverseBullets = 120;

    private float _reloadProgress;
    private bool _isReloading;

    [Header("반동")]
    public float RecoilAmount = 1.5f;

    public float ReloadProgress => _reloadProgress;
    public bool IsReloading => _isReloading;




    private void Start()
    {
        _time = _delay;
        _bulletCount = _bulletCountMax;
        _cameraRotate = Camera.main.GetComponent<CameraRotate>();
    }

    private void Update()
    {
        //1. 마우스 왼쪽 버튼이 눌린다면
        if (Input.GetMouseButton(0) && !IsReloading)
        {
            if (_bulletCount > 0)
            {
                if (_time >= _delay)
                {
                    //2. Ray를 생성하고 발사할 위치, 방향, 거리를 설정한다(쏜다)
                    Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);
                    //3.RaycastHit(충돌한 대상의 정보)를 저장할 변수를 생성한다.
                    RaycastHit hitInfo = new RaycastHit();
                    //4. 충돌했다면.. 피격 이펙트 표시
                    bool isHit = Physics.Raycast(ray, out hitInfo);
                    if (isHit)
                    {
                        //5. 충돌했다면...피격 이펙트 표시
                        Debug.Log(hitInfo.transform.name);

                        //파티클 생성과 플레이 방식
                        //1. Instantiate 방식 (+풀링) -> 한 화면에 여러가지 수정 후 여러개 그릴경우 / 새로 생성(메모리, CPU)
                        //2. 하나를 캐싱해두고 Play   -> 인스펙터 설정 그대로 그릴 경우 / 단점 : 재실행이므로 기존의 것이 삭제
                        //3. 하나를 캐싱해두고 Emit   -> 인스펙터 설정을 수정 후 그릴 경우
                        //ParticleSystem hitEffect = Instantiate(_hitEffect, hitInfo.point, Quaternion.identity);

                        _hitEffect.transform.position = hitInfo.point;
                        _hitEffect.transform.forward = hitInfo.normal;

                        _hitEffect.Play();

                       //태그랑 레이어 비교 안한 이유?
                       Monster monster = hitInfo.collider.gameObject.GetComponent<Monster>();
                        if (monster != null)
                        {
                            monster.TryTakeDamage(10);
                        }

                        _cameraRotate.AddRecoil(RecoilAmount);

                        //ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                        //emitParams.position = hitInfo.point;
                        //emitParams.rotation3D = Quaternion.LookRotation(hitInfo.normal).eulerAngles;

                        //_hitEffect.Emit(emitParams, 1); //Emit(커스텀할 정보, 분출할 갯수)
                    }
                    _bulletCount--;
                    _shootBullets++;
                    _time = 0f;
                }
                _time += Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && !IsReloading && ReverseBullets > 0)
        {
            StartCoroutine(Reload_Coroutine());
        }

        //Ray : 레이저(시작위치, 방향, 거리)
        //Raycast : 레이저를 발사
        //RaycastHit : 레이저가 물체와 충돌했다면 그 정보를 저장하는 구조체
    }
    private IEnumerator Reload_Coroutine()
    {
        _isReloading = true;
        _reloadProgress = 0f;

        float elapsedTime = 0f;

        // 장전 진행
        while (elapsedTime < ReloadTime)
        {
            elapsedTime += Time.deltaTime;
            _reloadProgress = elapsedTime / ReloadTime; // 0 ~ 1 사이의 값
            yield return null;
        }

        // 장전 완료
        _reloadProgress = 1f;
        _bulletCount = _bulletCountMax;
        ReverseBullets -= _shootBullets;
        _shootBullets = 0;

        // 바로 슬라이더 비우기
        _reloadProgress = 0f;
        _isReloading = false;
    }

}
