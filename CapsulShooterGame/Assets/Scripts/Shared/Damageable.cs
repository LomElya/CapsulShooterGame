using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Health))]
public class Damageable : MonoBehaviour
{

    [SerializeField] private Health _health;

    [Range(0, 1)]
    public float _sensibilityToSelfdamage = 0.5f;

    public void InflictDamage(float damage, GameObject damageSource)
    {
        if (_health)
        {
            var totalDamage = damage;

            // Уменьшить урон, если нанесен по самому себе
            if (_health.gameObject == damageSource)
            {
                totalDamage *= _sensibilityToSelfdamage;
            }

            // Урон
            _health.TakeDamage(totalDamage, damageSource);
        }
    }

}
