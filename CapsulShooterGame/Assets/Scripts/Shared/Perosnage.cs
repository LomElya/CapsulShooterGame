using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Actor))]
public abstract class Perosnage : MonoBehaviour
{
    [SerializeField] protected AssetActor _assetActor;

    [Header("Звук")]
    [SerializeField] protected AudioClip _damageTick;

    [Header("VFX")]
    [SerializeField] protected GameObject _deathVFX;
    [SerializeField] protected Transform _deathVFXSpawnPoint;

    [Header("Разное")]
    [SerializeField] protected Health _health;
    [SerializeField] protected Actor _actor;

    protected float _lastTimeDamaged = float.NegativeInfinity;

    protected bool _wasDamagedThisFrame;
    protected Collider[] _selfColliders;

    public Health Health => _health;

    public UnityAction onDamaged;

    protected virtual void Start()
    {
        _selfColliders = GetComponentsInChildren<Collider>();

        _health.OnDie += OnDie;
        _health.OnDamaged += OnDamaged;
    }

    protected virtual void Update()
    {
        _wasDamagedThisFrame = false;
    }

    protected virtual void OnDamaged(float damage, GameObject damageSource)
    {
        onDamaged?.Invoke();
        _lastTimeDamaged = Time.time;

        if (_damageTick && !_wasDamagedThisFrame)
            //AudioUtility.CreateSFX(_damageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

            _wasDamagedThisFrame = true;
    }

    protected virtual void OnDie()
    {
        // Заспавнить партикры при смерти
        if (_deathVFX != null)
        {
            var vfx = Instantiate(_deathVFX, _deathVFXSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        Destroy(gameObject);
    }

}
