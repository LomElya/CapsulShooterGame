using UnityEngine;

public class DoorPresenter : MonoBehaviour
{
    [SerializeField] DoorZoneTrigger _trigger;
    [SerializeField] Animator _animator;

    private bool onClick => Input.GetButton(StringConstant.BTN_Interaction);

    private bool IsOpen = true;
    private bool CanGo => (IsOpen && onClick);
    private bool onEnter = false;

    private const string On = nameof(On);
    private const string Off = nameof(Off);

    private void OnEnable()
    {
        _trigger.Enter += OnPlayerTriggerEnter;
        _trigger.Stay += OnPlayerTriggerStay;

        OnEnabled();
    }

    private void OnDisable()
    {
        _trigger.Enter -= OnPlayerTriggerEnter;
        _trigger.Exit -= OnPlayerTriggerStay;

        OnDisabled();
    }

    private void OnPlayerTriggerEnter(DoorOpener doorOpener)
    {
        if (onEnter || !CanGo)
            return;

        onEnter = true;

        EventManager.NexnLevel();

        OnEnter();
    }

    private void OnPlayerTriggerStay(DoorOpener doorOpener)
    {
        OnPlayerTriggerEnter(doorOpener);
    }

    public void OpedDoor(bool open, string status)
    {
        IsOpen = open;
        _animator?.SetTrigger(status);
    }

    protected virtual void OnEnabled() { }
    protected virtual void OnDisabled() { }
    protected virtual void OnEnter() { }
    protected virtual void OnExit() { }

}