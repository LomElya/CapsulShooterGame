using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMover : MonoBehaviour
{
    public enum AIState { Idle, Patrol, Follow, Attack, }

    [SerializeField] protected Animator _animator;
    [SerializeField] protected ParticleSystem[] RandomHitSparks;
    [SerializeField] protected ParticleSystem[] OnDetectVfx;

    [Header("Разное")]
    [SerializeField] protected Health _health;

    protected AIState _aiState { get; set; }

    protected float _timeStartedDetection;
    protected float _timeLostDetection;
    protected Quaternion _rotationWeaponForwardToPivot;
    protected Quaternion _previousPivotAimingRotation;
    protected Quaternion _pivotAimingRotation;

    protected const string _animOnDamagedParameter = "OnDamaged";
    protected const string _animisAttackdParameter = "isAttack";

    protected bool isGameStop;

    private void ContinueGame() => isGameStop = false;
    private void StopGame() => isGameStop = true;

    protected void OnEnable()
    {
        EventManager.OnStopGame.AddListener(StopGame);
        EventManager.OnContinueGame.AddListener(ContinueGame);
    }

    protected virtual void Start()
    {
        _health.OnDamaged += OnDamaged;
    }

    protected virtual void Update()
    {
        if (isGameStop)
            return;

        UpdateCurrentAiState();
    }

    public abstract void UpdateCurrentAiState();

    protected void OnDamaged(float dmg, GameObject source)
    {
        if (RandomHitSparks.Length > 0)
        {
            int n = Random.Range(0, RandomHitSparks.Length - 1);
            RandomHitSparks[n].Play();
        }

        if (_animator != null)
        {
            _animator?.SetTrigger(_animOnDamagedParameter);
        }
    }

}
