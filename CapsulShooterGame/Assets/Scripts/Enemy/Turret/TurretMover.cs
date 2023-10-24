using UnityEngine;

public class TurretMover : BaseMover
{
    [SerializeField] private Enemy _enemy;

    [Header("Основное")]
    [SerializeField] private Transform _turretPivot;
    [SerializeField] private Transform _turretAimPoint;
    [SerializeField] private float _aimRotationSharpness = 5f;
    [SerializeField] private float _lookAtRotationSharpness = 2.5f;
    [SerializeField] private float _detectionFireDelay = 1f;
    [SerializeField] private float _aimingTransitionBlendTime = 1f;

    [SerializeField] private AudioClip OnDetectSfx;

    //protected IDetectable _detectable;

    protected override void Start()
    {
        base.Start();/* 
        if (_enemy.TryGetComponent(out IDetectable detectable))
            _detectable = detectable;
        else
            Debug.LogError("Интерфейс IMoveble не найден у " + _enemy); */

        _enemy.onDetectedTarget += OnDetectedTarget;
        _enemy.onLostTarget += OnLostTarget;

        _rotationWeaponForwardToPivot =
               Quaternion.Inverse(_enemy.CurrentWeapon.Muzzle.rotation) * _turretPivot.rotation;

        _aiState = AIState.Idle;

        _timeStartedDetection = Mathf.NegativeInfinity;
        _previousPivotAimingRotation = _turretPivot.rotation;
    }

    private void LateUpdate()
    {
        if (isGameStop)
            return;

        UpdateTurretAiming();
    }

    public override void UpdateCurrentAiState()
    {
        switch (_aiState)
        {
            case AIState.Attack:
                if (_enemy.KnownDetectedTarget == null)
                    break;

                bool mustShoot = Time.time > _timeStartedDetection + _detectionFireDelay;
                // Целится на цель
                Vector3 directionToTarget =
                    (_enemy.KnownDetectedTarget.transform.position - _turretAimPoint.position).normalized;

                Quaternion offsettedTargetRotation =
                    Quaternion.LookRotation(directionToTarget) * _rotationWeaponForwardToPivot;

                _pivotAimingRotation = Quaternion.Slerp(_previousPivotAimingRotation, offsettedTargetRotation,
                    (mustShoot ? _aimRotationSharpness : _lookAtRotationSharpness) * Time.deltaTime);

                // Стрелять
                if (mustShoot)
                {
                    Vector3 correctedDirectionToTarget =
                        (_pivotAimingRotation * Quaternion.Inverse(_rotationWeaponForwardToPivot)) *
                        Vector3.forward;

                    _enemy.TryAttack(_turretAimPoint.position + correctedDirectionToTarget);
                    _animator?.SetTrigger(_animisAttackdParameter);
                }

                break;
        }
    }

    private void UpdateTurretAiming()
    {
        switch (_aiState)
        {
            case AIState.Attack:
                _turretPivot.rotation = _pivotAimingRotation;
                break;
            default:

                _turretPivot.rotation = Quaternion.Slerp(_pivotAimingRotation, _turretPivot.rotation,
                    (Time.time - _timeLostDetection) / _aimingTransitionBlendTime);
                break;
        }

        _previousPivotAimingRotation = _turretPivot.rotation;
    }

    private void OnDetectedTarget()
    {
        if (_aiState == AIState.Idle)
        {
            _aiState = AIState.Attack;
        }

        for (int i = 0; i < OnDetectVfx.Length; i++)
        {
            OnDetectVfx[i].Play();
        }

        if (OnDetectSfx)
        {
            //AudioUtility.CreateSFX(OnDetectSfx, transform.position, AudioUtility.AudioGroups.EnemyDetection, 1f);
        }

        _timeStartedDetection = Time.time;
    }

    private void OnLostTarget()
    {
        if (_aiState == AIState.Attack)
        {
            _aiState = AIState.Idle;
        }

        for (int i = 0; i < OnDetectVfx.Length; i++)
        {
            OnDetectVfx[i].Stop();
        }

        _timeLostDetection = Time.time;
    }
}