using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : Perosnage
{
    [SerializeField] protected Weapon _currentWeapon;
    [SerializeField] protected DetectionModule _detectionModule;

    [Header("Debug")]
    [SerializeField] protected Color _attackRangeColor = Color.red;
    [SerializeField] protected Color _detectionRangeColor = Color.blue;
    //Временная мера
    [SerializeField] protected bool _useDetect = true;

    public GameObject KnownDetectedTarget => _detectionModule.KnownDetectedTarget;
    public bool IsTargetInAttackRange => _detectionModule.IsTargetInAttackRange;
    public bool IsSeeingTarget => _detectionModule.IsSeeingTarget;
    public bool HadKnownTarget => _detectionModule.HadKnownTarget;
    public DetectionModule DetectionModule => _detectionModule;

    public UnityAction onAttack;
    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;

    public Weapon CurrentWeapon => _currentWeapon;

    protected EnemyManager _enemyManager;

    protected bool isGameStop;

    private void ContinueGame() => isGameStop = false;
    private void StopGame() => isGameStop = true;

    protected void OnEnable()
    {
        EventManager.OnStopGame.AddListener(StopGame);
        EventManager.OnContinueGame.AddListener(ContinueGame);
    }

    protected override void Start()
    {
        _enemyManager = FindObjectOfType<EnemyManager>();
        _enemyManager.RegisterEnemy(this);

        base.Start();

        _currentWeapon.ShowItem(true);
        _currentWeapon.SetOwner(this.gameObject);
        //Временная мера
        if (!_useDetect)
            return;

        _detectionModule.onDetectedTarget += OnDetectedTarget;
        _detectionModule.onLostTarget += OnLostTarget;

        onAttack += _detectionModule.OnAttack;
    }

    protected override void Update()
    {
        if (isGameStop)
            return;
            
        //Временная мера
        if (_useDetect)
            _detectionModule.HandleTargetDetection(_actor, _selfColliders);


        base.Update();
    }

    private void OnLostTarget()
    {
        onLostTarget?.Invoke();
    }

    private void OnDetectedTarget()
    {
        onDetectedTarget?.Invoke();
    }

    protected override void OnDamaged(float damage, GameObject damageSource)
    {
        // Проверить, является ли источник урона игрок
        if (damageSource && !damageSource.GetComponent<Enemy>())
        {
            base.OnDamaged(damage, damageSource);

            if (_useDetect)
                DetectionModule.OnDamaged(damageSource); //Временная мера
        }
    }

    protected override void OnDie()
    {
        _enemyManager.UnregisterEnemy(this);

        base.OnDie();
    }

    public bool TryAttack(Vector3 targetPosition)
    {
        OrientWeaponsTowards(targetPosition);

        bool didAttack = _currentWeapon.HandleUseInputs(false, true, false);

        if (didAttack && onAttack != null)
            onAttack.Invoke();

        return didAttack;
    }

    public void OrientWeaponsTowards(Vector3 lookPosition)
    {
        // Направить оружие на игрока
        Vector3 weaponForward = (lookPosition - _currentWeapon.Root.transform.position).normalized;
        _currentWeapon.transform.forward = weaponForward;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_useDetect) //Временная мера
            return;

        if (DetectionModule != null)
        {
            // Растояние обнаружения
            Gizmos.color = _detectionRangeColor;
            Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

            // Растояние атаки
            Gizmos.color = _attackRangeColor;
            Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
        }
    }
}
