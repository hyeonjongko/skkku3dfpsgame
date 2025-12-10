using UnityEngine;

public class BombFactory : MonoBehaviour
{
    private static BombFactory s_instance;
    public static BombFactory Instance => s_instance;

    [Header("폭탄 프리팹")]
    [SerializeField] private GameObject _bombPrefab;

    [Header("풀링")]
    public int PoolSize = 10;
    private GameObject[] _bombObjectPool;

    private void Awake()
    {
        if(s_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        s_instance = this;

        PoolInit();
    }

    private void PoolInit()
    {
        _bombObjectPool = new GameObject[PoolSize];

        for(int i = 0; i < PoolSize; ++i)
        {
            GameObject bombObject = Instantiate(_bombPrefab, transform);

            _bombObjectPool[i] = bombObject;

            bombObject.SetActive(false);
        }
    }
    public GameObject MakeBomb(Vector3 position)
    {
        for(int i = 0;i < PoolSize; ++i)
        {
            GameObject bombObject = _bombObjectPool[i];

            if(bombObject.activeInHierarchy == false)
            {
                bombObject.transform.position = position;
                bombObject.SetActive (true);

                return bombObject;
            }
        }
        return null;
    }
}
