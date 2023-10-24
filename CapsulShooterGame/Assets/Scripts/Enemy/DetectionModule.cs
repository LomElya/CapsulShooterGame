using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DetectionModule : MonoBehaviour
{
    [SerializeField] private Transform _detectionSourcePoint;
    [SerializeField] private float _detectionRange = 20f;
    [SerializeField] private float _attackRange = 10f;
    [SerializeField] private float _knownTargetTimeout = 4f;
    [SerializeField] private Animator _animator;

    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;

    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;

    public GameObject KnownDetectedTarget { get; private set; }
    public bool IsTargetInAttackRange { get; private set; }
    public bool IsSeeingTarget { get; private set; }
    public bool HadKnownTarget { get; private set; }

    protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

    private ActorsManager _actorsManager;

    private const string _animAttackParameter = "Attack";
    private const string _animOnDamagedParameter = "OnDamaged";

    private void Start()
    {
        _actorsManager = FindObjectOfType<ActorsManager>();
    }

    public void HandleTargetDetection(Actor actor, Collider[] selfColliders)
    {
        // Обработка таймера обнаружения
        if (KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) > _knownTargetTimeout)
        {
            KnownDetectedTarget = null;
        }

        // Найти ближайщего врага
        float sqrDetectionRange = _detectionRange * _detectionRange;
        IsSeeingTarget = false;
        float closestSqrDistance = Mathf.Infinity;
        foreach (Actor otherActor in _actorsManager.Actors)
        {
            if (otherActor.Affiliation != actor.Affiliation)
            {
                float sqrDistance = (otherActor.transform.position - _detectionSourcePoint.position).sqrMagnitude;
                if (sqrDistance < sqrDetectionRange && sqrDistance < closestSqrDistance)
                {
                    // Проверить наличие препятствий
                    RaycastHit[] hits = Physics.RaycastAll(_detectionSourcePoint.position,
                        (otherActor.AimPoint.position - _detectionSourcePoint.position).normalized, _detectionRange,
                        -1, QueryTriggerInteraction.Ignore);
                    RaycastHit closestValidHit = new RaycastHit();
                    closestValidHit.distance = Mathf.Infinity;
                    bool foundValidHit = false;
                    foreach (var hit in hits)
                    {
                        if (!selfColliders.Contains(hit.collider) && hit.distance < closestValidHit.distance)
                        {
                            closestValidHit = hit;
                            foundValidHit = true;
                        }
                    }

                    if (foundValidHit)
                    {
                        Actor hitActor = closestValidHit.collider.GetComponentInParent<Actor>();
                        if (hitActor == otherActor)
                        {
                            IsSeeingTarget = true;
                            closestSqrDistance = sqrDistance;

                            TimeLastSeenTarget = Time.time;
                            KnownDetectedTarget = otherActor.AimPoint.gameObject;
                        }
                    }
                }
            }
        }

        IsTargetInAttackRange = KnownDetectedTarget != null &&
                                Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <=
                                _attackRange;

        // Ивента обнаружения
        if (!HadKnownTarget &&
            KnownDetectedTarget != null)
        {
            OnDetect();
        }

        if (HadKnownTarget &&
            KnownDetectedTarget == null)
        {
            OnLostTarget();
        }

        // Запомнить, если цель была
        HadKnownTarget = KnownDetectedTarget != null;
    }

    public void OnLostTarget() => onLostTarget?.Invoke();

    public void OnDetect() => onDetectedTarget?.Invoke();

    public void OnDamaged(GameObject damageSource)
    {
        TimeLastSeenTarget = Time.time;
        KnownDetectedTarget = damageSource;

        if (_animator != null)
        {
            _animator?.SetTrigger(_animOnDamagedParameter);

        }
    }

    public void OnAttack()
    {
        if (_animator != null)
        {
            _animator?.SetTrigger(_animAttackParameter);
        }
    }
}