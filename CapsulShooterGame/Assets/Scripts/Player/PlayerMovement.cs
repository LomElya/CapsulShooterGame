using UnityEngine;
using Interfaces;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IModificationListener<float>
{

    [SerializeField] private CharacterAnimation _animation;
    [SerializeField] private Transform _playerModel;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _speedTurn = 0.1f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private float _groundDistance;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;

    private Rigidbody _rigidbody;
    private Vector3 _velocity;
    private float _speedRate = 1f;
    private float _flySpeedRate = 1f;
    private float _turnSmoothVelocityHolder;
    private Camera _camera;

    public bool IsMoving { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsGrounded { get; private set; }

    private void OnEnable()
    {
        EventManager.OnStartGame.AddListener(StartGame);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        GravityVelosity();
    }

    private void StartGame(LevelData levelData)
    {
        Teleport(levelData.InitialPosition);
    }

    public void Move(Vector3 direction)
    {
        if (_rigidbody == null)
            return;

        Vector3 move = Rotate(direction);

        _playerModel.LookAt(_playerModel.position + move);

        move *= _speed * _speedRate * _flySpeedRate;

        _rigidbody.velocity = new Vector3(move.x, _rigidbody.velocity.y, move.z);

        _animation?.SetSpeed(Mathf.Abs(move.magnitude));
    }

    public void Stop()
    {
        if (_rigidbody == null)
            return;

        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        _animation?.SetSpeed(0);
        IsMoving = false;
    }

    public void Jump()
    {
        if (_rigidbody == null)
            return;

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpForce * 1.5f, _rigidbody.velocity.z);
        IsJumping = true;
    }

    public void StopJump()
    {
        if (_rigidbody == null)
            return;

        if (_rigidbody.velocity.y > 0)
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.2f, _rigidbody.velocity.z);

        IsJumping = false;
    }

    public bool GroundCheck()
    {
        return IsGrounded = Physics.Raycast(_groundCheck.position, Vector3.down, _groundDistance, _groundMask);
    }

    public void Teleport(Vector3 position)
    {
        _rigidbody.position = position;
        _velocity *= 0;
        _rigidbody.velocity *= 0;
    }

    public void OnModificationUpdate(float value)
    {
        _speedRate = value;
    }

    private Vector3 Rotate(Vector3 direction)
    {
        float myAngleToLook = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, myAngleToLook, ref _turnSmoothVelocityHolder, _speedTurn);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, myAngleToLook, 0f) * Vector3.forward;

        return moveDirection;
    }

    private void GravityVelosity()
    {
        Vector3 gravity = Physics.gravity;

        if (_velocity.y < 0)
            _velocity += _gravityModifier * gravity * Time.deltaTime;
        else
            _velocity += gravity * Time.deltaTime;

        _velocity.x = 0f;
        _velocity.z = 0f;
    }
}
