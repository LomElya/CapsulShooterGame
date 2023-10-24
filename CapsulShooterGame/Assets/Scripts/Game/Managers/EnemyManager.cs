using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> Enemies { get; private set; }
    public int NumberOfEnemiesTotal { get; private set; }
    public int NumberOfEnemiesRemaining => Enemies.Count;

    void Awake()
    {
        Enemies = new List<Enemy>();
    }

    public void RegisterEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);

        NumberOfEnemiesTotal++;
    }

    public void UnregisterEnemy(Enemy enemyKilled)
    {
        int enemiesRemainingNotification = NumberOfEnemiesRemaining - 1;

        /* EnemyKillEvent evt = Events.EnemyKillEvent;
        evt.Enemy = enemyKilled.gameObject;
        evt.RemainingEnemyCount = enemiesRemainingNotification;
        EventManager.Broadcast(evt); */

        Enemies.Remove(enemyKilled);
    }
}