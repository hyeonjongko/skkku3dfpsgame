using UnityEngine;


//카메라 회전 기능
//마우스를 조작하면 카메라를 그 방향으로 회전하고 싶다.
public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 200f; // 0 ~ 360

    [Header("반동 설정")]
    public float MaxRecoilAngle = 30f; 
    public float ShakeAmount = 1.5f;   
    public float ShakeRecoverySpeed = 8f;
    private float _currentShakeX = 0f;

    // 유니티는  0 ~ 360도 체계이므로 우리가 따로 저장할 -360 ~ 360 체계로 누적할 변수
    private float _accumulationX = 0;
    private float _accumulationY = 0;
    private void Update()
    {
        if (Mathf.Abs(_currentShakeX) > 0.01f)
        {
            _currentShakeX = Mathf.Lerp(_currentShakeX, 0f, ShakeRecoverySpeed * Time.deltaTime);
        }
        else
        {
            _currentShakeX = 0f;
        }

        //게임이 시작하면 y축이 0도에서 시작 -> -1도
        if (!Input.GetMouseButton(2))
        {
            return;
        }
        // 1.마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //2. 마우스 입력을 누적한다.
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;
        _accumulationY += mouseY * RotationSpeed * Time.deltaTime;
        // 3.사람처럼 -90 ~ 90도 사이로 제한한다.
        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);

        // 4. 누적한 회전 방향으로 카메라 회전하기
        transform.eulerAngles = new Vector3(-_accumulationY, _accumulationX);

        //쿼터니언 : 사원수 : 쓰는 이유는 짐벌락 현상 방지
        //공부 : 짐벌락, 쿼터니언을 왜 쓰나

        //문제 : 잘 되긴 되는데 한번씩 세상이 뒤집어진다..

    }
    // 총 반동을 추가하는 메서드 (복구 없이 누적)
    public void AddRecoil(float recoilAmount)
    {
        // 위쪽 반동 누적 (복구 안됨)
        _accumulationY += recoilAmount;

        // 반동이 최대 각도를 넘지 않도록 제한
        if (_accumulationY < -MaxRecoilAngle)
        {
            _accumulationY = -MaxRecoilAngle;
        }

        // 전체 범위 제한 (-90 ~ 90)
        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);

        // 좌우 랜덤 쉐이크 추가 (복구됨)
        _currentShakeX += Random.Range(-ShakeAmount, ShakeAmount);

        // 즉시 카메라 회전 적용
        transform.eulerAngles = new Vector3(-_accumulationY, _accumulationX + _currentShakeX);
    }
}
