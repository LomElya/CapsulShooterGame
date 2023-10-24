using Unity.VisualScripting;
using UnityEngine;

public class CircuitPresenter : MonoBehaviour
{
    [SerializeField] CircuitZoneTrigger _trigger;
    [SerializeField] private Animator _animator;
    [SerializeField] private DoorPresenter _door;

    private const string On = nameof(On);
    private const string Off = nameof(Off);

    private bool onClick => Input.GetButton(StringConstant.BTN_Interaction);
    private bool isActive = false;

    private void OnEnable()
    {
        _trigger.Enter += OnPlayerTriggerEnter;
        _trigger.Stay += OnPlayerTriggerStay;

        OnEnabled();
    }
    private void Awake()
    {
        _door = FindFirstObjectByType<DoorPresenter>();
        _door.OpedDoor(isActive, Off);
    }

    private void OnDisable()
    {
        _trigger.Enter -= OnPlayerTriggerEnter;
        _trigger.Exit -= OnPlayerTriggerStay;

        OnDisabled();
    }

    private void OnPlayerTriggerEnter(DoorOpener doorOpener)
    {
        if (isActive || !onClick)
            return;

        isActive = true;

        _animator?.SetTrigger(On);
        _door.OpedDoor(isActive, On);

        OnEnter();
    }

    private void OnPlayerTriggerStay(DoorOpener doorOpener)
    {
        OnPlayerTriggerEnter(doorOpener);
        OnStay();

    }

    protected virtual void OnEnabled() { }
    protected virtual void OnDisabled() { }
    protected virtual void OnEnter() { }
    protected virtual void OnStay() { }
}