using UnityEngine;

//구조체 : 서로 다른 여러 데이터 타입의 변수들을 하나의 논리적 단위로 묶어 새로운 사용자 정의 자료형을 만드는 기능
public class Damage : MonoBehaviour
{
    public float Value;
    public Vector3 HitPoint;
    public Vector3 Normal;
    public Vector3 knockbackDirection;
    public GameObject Who;
    public bool Critical;

}
