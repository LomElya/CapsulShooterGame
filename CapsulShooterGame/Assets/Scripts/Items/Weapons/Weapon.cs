using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public enum WeaponAttackType
    {
        Manual,
        Automatic,
        Charge,
    }

    [SerializeField] protected float _delayBetweenAttack = 0.5f;
    public Vector3 MuzzleWorldVelocity { get; set; }

    public abstract WeaponAttackType _attackType { get; }
    public abstract WeaponRangeType RangeType { get; }

    protected bool isGameStop;

    protected void ContinueGame() => isGameStop = false;
    protected void StopGame() => isGameStop = true;

    protected readonly Dictionary<string, CustomPool<ProjectileBase>> _pools =
       new Dictionary<string, CustomPool<ProjectileBase>>();

    protected void OnEnable()
    {
        EventManager.OnStopGame.AddListener(StopGame);
        EventManager.OnContinueGame.AddListener(ContinueGame);
        // EventManager.OnStartGame.AddListener(RemovePool);
    }

    protected CustomPool<ProjectileBase> GetPool(ProjectileBase projectile)
    {
        var objectTypeStr = projectile.GetType().ToString();
        CustomPool<ProjectileBase> pool;

        // Создать новый пул, если такого нет
        if (!_pools.ContainsKey(objectTypeStr))
        {
            pool = new CustomPool<ProjectileBase>(projectile, 1);
            _pools.Add(objectTypeStr, pool);
        }
        // Если пул есть, возвращаем его
        else
        {
            pool = _pools[objectTypeStr];
        }

        return pool;
    }

    protected void Dispose(ProjectileBase projectile)
    {
        var pool = GetPool(projectile);
        pool.Release(projectile);
    }
}