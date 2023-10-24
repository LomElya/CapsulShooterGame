using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 1f;
    [SerializeField] private float _currentHealth;

    private bool _isDead => _currentHealth <= 0;
    public bool SelfDestruction { get; set; }

    public bool CanPickup() => _currentHealth < _maxHealth;

    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction<float> OnHealed;
    public UnityAction OnDie;

    private void OnEnable()
    {
        EventManager.OnStartHardMode.AddListener(SetSelfDestruction);
    }
    private void Start()
    {
        _currentHealth = _maxHealth;

        LevelController levelController = FindAnyObjectByType<LevelController>();
        if (levelController != null)
        {
            SelfDestruction = levelController.IsHardMode;
        }

    }

    public virtual void Heal(float healAmount)
    {
        float healthBefore = _currentHealth;
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        float trueHealAmount = _currentHealth - healthBefore;
        if (trueHealAmount > 0f)
            OnHealed?.Invoke(trueHealAmount);
    }
    public virtual void TakeDamage(float damage, GameObject damageSource)
    {
        float healthBefore = _currentHealth;
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        float trueDamageAmount = healthBefore - _currentHealth;

        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }

        HandleDeath();
    }

    public void Kill()
    {
        _currentHealth = 0f;

        OnDamaged?.Invoke(_maxHealth, null);
        HandleDeath();
    }

    public void SelfKill()
    {
        if (!SelfDestruction)
            return;

        Kill();
    }

    private void SetSelfDestruction(bool status)
    {
        SelfDestruction = status;
    }

    private void HandleDeath()
    {
        if (!_isDead)
            return;

        if (_isDead)
            OnDie?.Invoke();
    }
}