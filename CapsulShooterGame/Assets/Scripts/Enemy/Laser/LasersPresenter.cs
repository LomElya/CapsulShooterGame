using System;
using UnityEngine;
using UnityEngine.Events;

public class LasersPresenter : Weapon
{
    [SerializeField] private Laser _firstLaser;
    [SerializeField] private Laser _secondLaser;
    [SerializeField] private ProjectileBase _pojectilePrefab;
    [SerializeField] private bool _isShoot = false;

    public UnityAction OnShoot;
    public event Action OnShootProcessed;

    protected override string _animUseParameter => "isAttack";

    private Transform _firstMuzzleTransform => _firstLaser.Muzzle.transform;
    private Transform _secondMuzzleTransform => _secondLaser.Muzzle.transform;

    public override WeaponAttackType _attackType => WeaponAttackType.Automatic;
    public override WeaponRangeType RangeType => WeaponRangeType.Range;

    private void Update()
    {

        if (_isShoot)
        {
            _isShoot = false;
            Vector3 shotDirection = GetShotDirectionWithinSpread(_firstLaser.Muzzle);
            ProjectileBase projectile = Instantiate(_pojectilePrefab, _firstLaser.Muzzle.position, Quaternion.LookRotation(shotDirection), _firstLaser.Muzzle);
            SetOwner(this.gameObject);
            projectile.Shoot(this);
        }
        /* if (_lastTimeUse + _delayBetweenAttack < Time.time)
        {
            ProjectileLaser projectile = Instantiate(_pojectilePrefab, _firstLaser.Muzzle);
            Destroy(projectile, _delayBetweenAttack);
        } */

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
        {
            // Луч лазера
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_firstLaser.Muzzle.position, _secondLaser.Muzzle.position);
        }
    }
}