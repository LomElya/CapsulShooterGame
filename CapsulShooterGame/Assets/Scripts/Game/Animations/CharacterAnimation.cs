using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    //[SerializeField] private StackCharacter _playerStack;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        OnAwake();
    }
    private void OnEnable()
    {
        /* _playerStack.Added += OnAdded;
        _playerStack.BecameEmpty += OnBecameEmpty; */
    }

    private void OnDisable()
    {
        /* _playerStack.Added -= OnAdded;
               _playerStack.BecameEmpty -= OnBecameEmpty; */
    }

    public void SetSpeed(float speed)
    {
        _animator?.SetFloat(AnimationParams.Speed, speed);
    }

    protected void OnAwake() { }

    private static class AnimationParams
    {
        public static readonly string Speed = nameof(Speed);
        public static readonly string Idle = nameof(Idle);
    }
}
