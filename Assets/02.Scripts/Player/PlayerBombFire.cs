using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerBombFire : MonoBehaviour
{
    //마우스 오른쪽 버튼을 누르면 카메라(플레이어가 바라보는 방향으로 던지고 싶다.

    //필요 속성
    //- 발사 위치
    //- 발사할 폭탄
    //- 던질 힘

    [SerializeField] private Transform _fireTransform;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _throwPower = 15f;

    [SerializeField] private int _bombCount = 5;
    public int BombCount => _bombCount;

    private BombFactory _bombFactory;


    private void Start()
    {
        _bombFactory = GameObject.Find("BombFactory").GetComponent<BombFactory>();
        Debug.Log($"남은 폭탄 수 : {_bombCount}");
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            _bombCount--;
            if(_bombCount >= 0)
            {
                
                GameObject bomb = _bombFactory.MakeBomb(_fireTransform.position);
                //Bomb bomb = Instantiate(_bombPrefab, _fireTransform.position, Quaternion.identity);
                Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
                rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
                Debug.Log($"남은 폭탄 수 : {_bombCount}");
            }
            else if(_bombCount < 0)
            {
                _bombCount = 0;
                Debug.Log($"남은 폭탄이 없습니다.");
            }
            
        }
    }

}
