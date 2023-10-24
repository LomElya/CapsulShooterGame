using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
    // Для подписки через AddListener()
    public static readonly UnityEvent<LevelData> OnStartGame = new UnityEvent<LevelData>();
    public static readonly UnityEvent OnStopGame = new UnityEvent();
    public static readonly UnityEvent OnContinueGame = new UnityEvent();
    public static readonly UnityEvent OnNextLevel = new UnityEvent();
    public static readonly UnityEvent<bool> OnStartHardMode = new UnityEvent<bool>();
    public static readonly UnityEvent OnWinGame = new UnityEvent();
    public static readonly UnityEvent<Vector3> OnPlayerDeath = new UnityEvent<Vector3>();
    public static readonly UnityEvent<Weapon, ProjectileBase, Vector3, Quaternion> OnShoot = new UnityEvent<Weapon, ProjectileBase, Vector3, Quaternion>();
    public static readonly UnityEvent<ProjectileBase> OnDispose = new UnityEvent<ProjectileBase>();

    // Вызов ивента
    public static void StartGame(LevelData levelData)
    {
        OnStartGame.Invoke(levelData);
    }

    public static void StopGame()
    {
        OnStopGame.Invoke();
    }
    public static void ContinueGame()
    {
        OnContinueGame.Invoke();
    }

    public static void NexnLevel()
    {
        OnNextLevel.Invoke();
    }

    public static void StartHardMode(bool status)
    {
        OnStartHardMode.Invoke(status);
    }

    public static void WinGame()
    {
        OnWinGame.Invoke();
    }

    public static void PlayerDeath(Vector3 position)
    {
        OnPlayerDeath.Invoke(position);
    }

    public static void Shoot(Weapon weapon, ProjectileBase projectile, Vector3 position, Quaternion rotation)
    {
        OnShoot.Invoke(weapon, projectile, position, rotation);
    }

    public static void Dispose(ProjectileBase projectile)
    {
        OnDispose.Invoke(projectile);
    }
}
