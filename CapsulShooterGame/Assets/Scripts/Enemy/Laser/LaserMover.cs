using UnityEngine;

public class LaserMover : BaseMover
{
    [SerializeField] private Enemy _enemy;
    protected IMoveble _moveble;
    protected override void Start()
    {
        base.Start();

        if (_enemy.TryGetComponent(out IMoveble moveble))
            _moveble = moveble;
        else
            Debug.LogError("Интерфейс IMoveble не найден у " + _enemy);

        _aiState = AIState.Patrol;
    }

    public override void UpdateCurrentAiState()
    {
        switch (_aiState)
        {
            case AIState.Patrol:
                _moveble.UpdatePathDestination();
                _moveble.SetNavDestination(_moveble.GetDestinationOnPath());
                break;
        }
    }
}