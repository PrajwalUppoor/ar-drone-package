using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInput : MonoBehaviour
{
    [SerializeField] private DroneController controller;
    
    // Existing actions from InputSystem_Actions
    private InputAction moveAction;
    
    // New actions we'll add
    private InputAction throttleAction;
    private InputAction yawAction;

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private IndustrialMachine machineController;
    public bool controllingMachine = false;

    [Header("Mobile Controls")]
    [SerializeField] private Joystick leftJoystick; // Throttle / Yaw
    [SerializeField] private Joystick rightJoystick; // Horizontal Movement

    private void Awake()
    {
        if (controller == null) controller = GetComponent<DroneController>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        
        // Find machine if not assigned
        if (machineController == null) machineController = FindFirstObjectByType<IndustrialMachine>();

        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            throttleAction = playerInput.actions["Throttle"];
            yawAction = playerInput.actions["Yaw"];

            moveAction?.Enable();
            throttleAction?.Enable();
            yawAction?.Enable();
        }
    }

    private float logTimer = 0f;
    private void Update()
    {
        // Log position (only for drone)
        if (!controllingMachine && controller != null)
        {
            logTimer += Time.deltaTime;
            if (logTimer >= 5f)
            {
                Debug.Log($"Drone Status - Position: {controller.transform.position}");
                logTimer = 0f;
            }
        }

        Vector2 move = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        
        if (move == Vector2.zero && Keyboard.current != null)
        {
            float x = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            float y = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
            move = new Vector2(x, y);
        }
        if (rightJoystick != null && rightJoystick.Direction != Vector2.zero)
        {
            move += rightJoystick.Direction;
        }

        float throttle = throttleAction != null ? throttleAction.ReadValue<float>() : 0f;
        if (throttle == 0 && Keyboard.current != null)
        {
            throttle = (Keyboard.current.spaceKey.isPressed ? 1f : (Keyboard.current.leftShiftKey.isPressed ? -1f : 0f));
        }
        if (leftJoystick != null)
        {
            throttle += leftJoystick.Vertical;
        }

        float yaw = yawAction != null ? yawAction.ReadValue<float>() : 0f;
        if (yaw == 0 && Keyboard.current != null)
        {
            yaw = (Keyboard.current.qKey.isPressed ? -1f : (Keyboard.current.eKey.isPressed ? 1f : 0f));
        }
        if (leftJoystick != null)
        {
            yaw += leftJoystick.Horizontal;
        }

        // Send inputs to the active target
        if (controllingMachine && machineController != null)
        {
            machineController.SetInputs(move, throttle, yaw);
            // Reset drone inputs to zero so it doesn't drift
            if (controller != null) controller.SetInputs(Vector2.zero, 0, 0);
        }
        else if (controller != null)
        {
            controller.SetInputs(move, throttle, yaw);
            // Reset machine input to zero
            if (machineController != null) machineController.SetInputs(Vector2.zero, 0, 0);
        }
    }

    public void SetMachineTarget(IndustrialMachine machine)
    {
        machineController = machine;
    }
}
