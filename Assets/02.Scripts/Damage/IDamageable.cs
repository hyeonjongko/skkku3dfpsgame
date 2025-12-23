using UnityEngine;

//인터페이스 : 클래스간의 약속
public interface IDamageable
{
    //IDamageable 약속을 지켜야하는 클래스는 무조건 
    public bool TryTakeDamage(Damage damage);
}
