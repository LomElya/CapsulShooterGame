using System;
using System.Collections;
using UnityEngine;

public class PressbuttonZoneTrigger : Trigger<DoorOpener>
{
    [SerializeField] private Door[] _openingDoors;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _delayBeforeClose = 1.5f;

    public bool CanOpen => true;
    public bool IsOpen = false;

    private Coroutine _waitClose;

    private const string Open = nameof(Open);
    private const string Close = nameof(Close);

    protected override void OnEnter(DoorOpener triggered)
    {
        if (!CanOpen)
            return;

        if (_waitClose != null)
            StopCoroutine(_waitClose);

        if (IsOpen)
            return;

        //IsOpen = true;
        StartAnimations(Open, true);
    }

    protected override void OnStay(DoorOpener triggered)
    {
        if (CanOpen && !IsOpen)
            OnEnter(triggered);
    }

    protected override void OnExit(DoorOpener triggered)
    {

        if (_waitClose != null)
            StopCoroutine(_waitClose);

        _waitClose = StartCoroutine(WaitBefore(_delayBeforeClose, () =>
        {
            if (!IsOpen)
                return;

            StartAnimations(Close, false);
        }));
    }

    private IEnumerator WaitBefore(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);

        action?.Invoke();
    }

    private void StartAnimations(string status, bool isOpen)
    {
        _animator?.SetTrigger(status);
        IsOpen = isOpen;

        foreach (Door door in _openingDoors)
        {
            door.Animator?.SetTrigger(status);
            SetOpenDoors(door, isOpen);
        }
    }

    private void SetOpenDoors(Door door, bool isOpen)
    {
        door._isOpen = isOpen;
    }
}