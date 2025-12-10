using Unity.VisualScripting;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    //목표 : 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총알을 날리고 싶다)
    [SerializeField] private Transform _fireTransform;
    private void Update()
    {
        //1. 마우스 왼쪽 버튼이 눌린다면
        if(Input.GetMouseButtonDown(0))
        {
            //2. Ray를 생성하고 발사할 위치, 방향, 거리를 설정한다(쏜다)
            Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);
            //3.RaycastHit(충돌한 대상의 정보)를 저장할 변수를 생성한다.
            RaycastHit hitInfo = new RaycastHit();
            //4. 충돌했다면.. 피격 이펙트 표시
            bool isHit = Physics.Raycast(ray, out  hitInfo);
            if(isHit)
            {
                //5. 충돌했다면...피격 이펙트 표시
                Debug.Log(hitInfo.transform.name);
            }
        }

        //Ray : 레이저(시작위치, 방향, 거리)
        //Raycast : 레이저를 발사
        //RaycastHit : 레어저를 맞았는지 맞지 않았는지 판단.
    }


}
