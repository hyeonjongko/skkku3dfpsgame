using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //도메인 : 특정 분야의 지식
    public ConsumableStat Health;
    public ConsumableStat Stamina;
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;
    public ValueStat JumpPower;

    private void Start()
    {
        Health.Initialize();
        Stamina.Initialize();
    }
    private void Update()
    {
        float dealtaTime = Time.deltaTime;

        Health.Regenerate(dealtaTime);
        Stamina.Regenerate(dealtaTime);
    }
}
