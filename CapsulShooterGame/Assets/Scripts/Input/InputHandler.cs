using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] Health _health;

    public bool LastFrameMoving { get; private set; }
    public float Horizontal => Input.GetAxisRaw(StringConstant.BTN_Horizontal);
    public float Vertical => Input.GetAxisRaw(StringConstant.BTN_Vertical);
    public Vector2 Direction { get { return new Vector3(Horizontal, Vertical); } }

    public bool Moving => Input.GetButton(StringConstant.BTN_Horizontal) || Input.GetButton(StringConstant.BTN_Vertical);
    public bool isJumping => Input.GetButton(StringConstant.BTN_Jump);

    public bool StoppedMoving => !Moving && LastFrameMoving;
    public bool StoppedJumping => !isJumping;

    private bool _useInputWasHeld;

    public bool _selfDestruction => Input.GetButton(StringConstant.BTN_SelfDestruction);

    private bool isGameStop;

    private void OnEnable()
    {
        EventManager.OnStopGame.AddListener(StopGame);
        EventManager.OnContinueGame.AddListener(ContinueGame);
    }

    private void OnDisable()
    {
        _playerMovement?.Stop();
    }

    private void Update()
    {
        if (isGameStop)
            return;

        if (!Moving)
            _playerMovement.Stop();
        else
        {
            if (!isJumping)
                _playerMovement.OnModificationUpdate(1f);

            Vector3 move = new Vector3(Direction.x, 0, Direction.y);

            _playerMovement.Move(move);
        }

        if (isJumping && _playerMovement.GroundCheck())
        {
            _playerMovement.Jump();

            if (Moving)
                _playerMovement.OnModificationUpdate(1.2f);
        }
        else if (StoppedJumping)
            _playerMovement.StopJump();

        if (_selfDestruction)
            _health.SelfKill();

    }

    private void LateUpdate()
    {
        LastFrameMoving = Moving;
        _useInputWasHeld = GetUseInputHeld();
    }

    public bool GetUseInputHeld()
    {
        if (isGameStop)
            return false;

        return Input.GetButton(StringConstant.BTN_Attack);
    }

    public bool GetUseInputDown()
    {
        return GetUseInputHeld() && !_useInputWasHeld;
    }

    public bool GetUseInputReleased()
    {
        return !GetUseInputHeld() && _useInputWasHeld;
    }

    public int GetSwitchItemInput()
    {
        if (isGameStop)
            return 0;

        if (Input.GetAxis("NextItem") > 0f)
            return -1;
        else if (Input.GetAxis("NextItem") < 0f)
            return 1;

        return 0;
    }

    public int GetSelectItemInput()
    {
        if (isGameStop)
            return 0;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            return 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            return 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            return 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            return 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            return 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            return 6;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            return 7;
        else
            return 0;

    }

    private void ContinueGame() => isGameStop = false;
    private void StopGame() => isGameStop = true;
}
