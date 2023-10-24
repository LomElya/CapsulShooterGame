using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Основное")]
    [SerializeField] protected float _radius = 0.01f;
    [SerializeField] protected Transform _root;
    [SerializeField] protected Transform _tip;
    [SerializeField] protected float _maxLifeTime = 5f;
    public GameObject _impactVfx;
    [SerializeField] protected float _impactVfxLifetime = 5f;
    [SerializeField] protected float _impactVfxSpawnOffset = 0.1f;
    [SerializeField] protected AudioClip _impactSfxClip;

    [Space(15)]
    [SerializeField] protected LayerMask _hittableLayers = -1;
    [field: SerializeField] public MeshRenderer Mesh { get; private set; }
    [field: SerializeField] public Material Material { get; private set; }
    [Header("Движение")]
    [SerializeField] protected float _speed = 20f;

    [Header("Урон")]
    [SerializeField] protected float _damage = 40f;

    [Header("Debug")]
    [SerializeField] protected Color _radiusColor = Color.cyan * 0.2f;

    protected ProjectileBase _projectileBase => this;
    protected Vector3 _last_rootPosition;
    protected Vector3 _velocity;
    protected bool _hasTrajectoryOverride;
    protected Vector3 _trajectoryCorrectionVector;
    protected Vector3 _consumedTrajectoryCorrectionVector;
    protected List<Collider> _ignoredColliders;
    protected Actor _actor;

    protected const QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Collide;

    public GameObject Owner { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }
    public Vector3 InheritedMuzzleVelocity { get; private set; }

    public UnityAction OnShoot;
    public UnityAction<ProjectileBase> OnDispose;

    public void Shoot(Weapon weapon)
    {
        Owner = weapon.Owner;
        InitialPosition = transform.position;
        InitialDirection = transform.forward;
        InheritedMuzzleVelocity = weapon.MuzzleWorldVelocity;
        _actor = Owner.GetComponent<Actor>();
        //_projectileBase = this;

        OnShoot?.Invoke();
    }

    protected virtual bool IsHitValid(RaycastHit hit)
    {
        // Игнорировать, если нет триггера и нет компонента "Damageable"
        if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
        {
            return false;
        }

        // Игнорировать, если найден свой коллайдер
        if (_ignoredColliders != null && _ignoredColliders.Contains(hit.collider))
        {
            return false;
        }

        //Игнорировать, если в одной принадлежности
        Actor actor = hit.collider.GetComponentInParent<Actor>();
        if (actor && actor.Affiliation == _actor.Affiliation)
        {
            return false;
        }


        return true;
    }

    protected virtual void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        // Урон
        Damageable damageable = collider.GetComponent<Damageable>();
        if (damageable)
        {
            damageable.InflictDamage(_damage, _projectileBase.Owner);
        }

        // VFX
        if (_impactVfx)
        {
            GameObject _impactVfxInstance = Instantiate(_impactVfx, point + (normal * _impactVfxSpawnOffset),
                Quaternion.LookRotation(normal));
            if (_impactVfxLifetime > 0)
            {
                Destroy(_impactVfxInstance.gameObject, _impactVfxLifetime);
            }
        }

        // SFX
        if (_impactSfxClip)
        {
            AudioUtility.CreateSFX(_impactSfxClip, point, AudioUtility.AudioGroups.Shoot, 1f, 3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _radiusColor;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
