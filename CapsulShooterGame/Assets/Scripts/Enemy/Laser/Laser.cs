using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Laser : Weapon
{
    [SerializeField] private ProjectileBase _projectilePrefab;

    public UnityAction OnShoot;
    public event Action OnShootProcessed;

    protected override string _animUseParameter => "isAttack";

    public override WeaponAttackType _attackType => WeaponAttackType.Manual;
    public override WeaponRangeType RangeType => WeaponRangeType.Range;

    private void Update()
    {
        if (isGameStop)
            return;

        if (Time.deltaTime > 0)
        {
            MuzzleWorldVelocity = (_muzzle.position - _lastMuzzlePosition) / Time.deltaTime;
            _lastMuzzlePosition = _muzzle.position;
        }
    }

    public override bool HandleUseInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        _wantsToUse = inputDown || inputHeld;

        switch (_attackType)
        {
            case WeaponAttackType.Manual:
                {
                    if (inputDown)
                        return TryUse();

                    return false;
                }
            case WeaponAttackType.Automatic:
                {
                    if (inputHeld)
                        return TryUse();

                    return false;
                }
            default:
                return false;
        }
    }

    public override bool TryUse()
    {
        if (_lastTimeUse + _delayBetweenAttack < Time.time)
        {
            HandleUse();
            return true;
        }
        return false;
    }

    public override void HandleUse()
    {
        Vector3 shotDirection = GetShotDirectionWithinSpread(_muzzle);

        //ProjectileBase newProjectile = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.LookRotation(shotDirection), _muzzle);
        ProjectileBase projectile = _projectilePrefab;
        var pool = GetPool(projectile);

        ProjectileBase newProjectile = pool.Get();

        newProjectile.transform.position = _muzzle.position;
        newProjectile.transform.rotation = Quaternion.LookRotation(shotDirection);
        newProjectile.transform.parent = _muzzle;

        newProjectile.OnDispose += Dispose;
        newProjectile.Shoot(this);

        if (_muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(_muzzleFlashPrefab, _muzzle.position, _muzzle.rotation, _muzzle.transform);

            if (_unparentMuzzleFlash)
            {
                muzzleFlashInstance.transform.SetParent(null);
            }

            Destroy(muzzleFlashInstance, 2f);
        }
        _lastTimeUse = Time.time;

        if (_useSound)
        {
            _useAudioSource?.PlayOneShot(_useSound);
        }

        // Триггер анимации, если анимация есть
        if (_animator != null)
            _animator?.SetTrigger(_animUseParameter);

        OnShoot?.Invoke();
        OnShootProcessed?.Invoke();
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = 0 / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }

    private void OnDrawGizmosSelected()
    {

        // Луч направления оружия
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_muzzle.position, transform.forward * 30f);
    }
}