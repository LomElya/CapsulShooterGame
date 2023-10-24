using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public Animator Animator => _animator;
    public bool _isOpen = false;
}