using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Transform _parent;

    private readonly Dictionary<string, CustomPool<ProjectileBase>> _pools =
      new Dictionary<string, CustomPool<ProjectileBase>>();

    private readonly List<ProjectileBase> _projectilesList = new List<ProjectileBase>();

    private void OnEnable()
    {
        EventManager.OnStartGame.AddListener(StartGame);
        EventManager.OnShoot.AddListener(Spawn);
        EventManager.OnDispose.AddListener(Dispose);
    }

    private void Spawn(Weapon weapon, ProjectileBase projectilePrefab, Vector3 position, Quaternion rotation)
    {
        ProjectileBase projectile = projectilePrefab;
        CustomPool<ProjectileBase> pool = GetPool(projectile);

        ProjectileBase newProjectile = pool.Get();

        newProjectile._impactVfx = projectile._impactVfx;
        newProjectile.Mesh.material = projectile.Mesh.sharedMaterial;

        newProjectile.transform.position = position;
        newProjectile.transform.rotation = rotation;
        newProjectile.transform.parent = _parent;

        newProjectile.Shoot(weapon);

        if (!_projectilesList.Contains(newProjectile))
            _projectilesList.Add(newProjectile);
    }

    private void Dispose(ProjectileBase projectile)
    {
        var pool = GetPool(projectile);
        pool.Release(projectile);

       /*  if (_projectilesList.Contains(projectile))
            _projectilesList.Remove(projectile); */
    }

    private CustomPool<ProjectileBase> GetPool(ProjectileBase projectile)
    {
        var objectTypeStr = projectile.GetType().ToString();
        CustomPool<ProjectileBase> pool;

        // Создать новый пул, если такого нет
        if (!_pools.ContainsKey(objectTypeStr))
        {
            pool = new CustomPool<ProjectileBase>(projectile, 1);
            _pools.Add(objectTypeStr, pool);
            //_projectilesList.Add(pool);
        }
        // Если пул есть, возвращаем его
        else
        {
            pool = _pools[objectTypeStr];
        }

        return pool;
    }

    private void StartGame(LevelData levelData)
    {
        if (_pools != null)
            _pools.Clear();

        if (_projectilesList == null)
            return;

        foreach (var projectile in _projectilesList)
        {
            Destroy(projectile.gameObject);
        }
        _projectilesList.Clear();

    }

}
