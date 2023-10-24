using System;
using UnityEngine;
using UnityEngine.Events;

public class Pistol : Weapon
{
    //Разброс пуль
    [SerializeField] private float _bulletSpreadAngle = 0f;
    [SerializeField] private int _bulletsPerShot = 1;

    [Range(0f, 2f)]
    [SerializeField] private float _recoilForce = 1;
    [SerializeField] private ProjectileBase _projectilePrefab;

    public UnityAction OnShoot;
    public event Action OnShootProcessed;

    protected override string _animUseParameter => "isAttack";

    public float RecoilForce => _recoilForce;

    public override WeaponRangeType RangeType => WeaponRangeType.Range;
    public override WeaponAttackType _attackType => WeaponAttackType.Automatic;

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
        int bulletsPerShotFinal = _bulletsPerShot;

        for (int i = 0; i < bulletsPerShotFinal; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(_muzzle);
            
            EventManager.Shoot(this, _projectilePrefab, _muzzle.position, Quaternion.LookRotation(shotDirection));
        }

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
        float spreadAngleRatio = _bulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }
}
