using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour, IDamageable
{
    Rigidbody rb;

    public LayerMask DamageLayer;

    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ValueStat _damage;
    [SerializeField] private ValueStat _explosionRadius;

    [SerializeField] private ParticleSystem _explosionParticlePrefab;

    private float _force = 1500.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _health.Initialize();
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (_health.Value <= 0) return false;
        _health.Decrease(damage.Value);

        if (_health.Value <= 0)
        {
            StartCoroutine(Explode_Coroutine());
        }
        return true;
    }
    private IEnumerator Explode_Coroutine()
    {

        rb.AddForce(Vector3.up * _force);
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * 90f);

        ParticleSystem explosionParticle = Instantiate(_explosionParticlePrefab);
        _explosionParticlePrefab.transform.position = this.transform.position;
        _explosionParticlePrefab.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius.Value, DamageLayer);

        Damage damage = new Damage()
        {
            Value = _damage.Value,
            HitPoint = transform.position,
            Who = this.gameObject,
            Critical = false,
        };

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TryTakeDamage(damage);
            }

            //if (colliders[i].TryGetComponent<Monster>(out Monster monster))
            //{
            //    monster.TryTakeDamage(damage);
            //}
            //if (colliders[i].TryGetComponent<Drum>(out Drum drum))
            //{
            //    drum.TryTakeDamage(damage);
            //}

        }

        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);
    }
}

