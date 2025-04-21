using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions actions;

    Vector2 inputVec;
    public Vector2 InputVec { get => inputVec; }

    bool isAttack = false;
    public bool IsAttack { get => isAttack; }

    bool isRoll = false;
    public bool IsRoll { get => isRoll; }

    private void Awake()
    {
        actions = new PlayerInputActions();

        actions.Player.Move.performed += Move_performed;
        actions.Player.Move.canceled += Move_canceled;
        actions.Player.Attack.performed += Attack_performed;
        actions.Player.Attack.canceled += Attack_canceled;
        actions.Player.Roll.performed += Roll_performed;
        actions.Player.Roll.canceled += Roll_canceled;
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Move.Enable();
        actions.Player.Attack.Enable();
        actions.Player.Roll.Enable();
    }

    private void OnDisable()
    {
        actions.Player.Roll.Disable();
        actions.Player.Attack.Disable();
        actions.Player.Move.Disable();

        actions.Player.Disable();
    }

    private void Roll_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isRoll = false;
    }

    private void Roll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isRoll = true;
    }

    private void Attack_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isAttack = false;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isAttack = true;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector2 value = obj.ReadValue<Vector2>();
        inputVec = value;
    }

    private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        inputVec = Vector2.zero;
    }
}