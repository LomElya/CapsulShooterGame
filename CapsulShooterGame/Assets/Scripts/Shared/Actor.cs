using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] private int _affiliation;
    [SerializeField] private Transform _aimPoint;

    private ActorsManager _actorsManager;

    public int Affiliation => _affiliation;
    public Transform AimPoint => _aimPoint;

    private void Start()
    {
        _actorsManager = GameObject.FindObjectOfType<ActorsManager>();

        if (!_actorsManager.Actors.Contains(this))
            _actorsManager.Actors.Add(this);
    }

    private void OnDestroy()
    {
        if (_actorsManager)
            _actorsManager.Actors.Remove(this);
    }
}
