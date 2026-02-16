using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Flight Settings")]
    [SerializeField] private float liftForce = 15f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float tiltAmount = 20f;
    [SerializeField] private float tiltSpeed = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private float throttleInput;
    private float yawInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        // Increase damping to help with stabilization and prevent excessive drifting
        rb.linearDamping = 2f;
        rb.angularDamping = 4f;
    }

    public void SetInputs(Vector2 move, float throttle, float yaw)
    {
        moveInput = move;
        throttleInput = throttle;
        yawInput = yaw;
    }

    private void FixedUpdate()
    {
        HandleLift();
        HandleMovement();
        HandleRotation();
        HandleTilt();
    }

    [Header("Safety Settings")]
    [SerializeField] private float minHeightFromGround = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    private void HandleLift()
    {
        // Counteract gravity (9.81) to maintain hover by default
        float gravityOffset = Physics.gravity.magnitude;
        float verticalForce = gravityOffset + (throttleInput * liftForce);
        
        // Use all layers if groundLayer is set to Nothing (0)
        LayerMask effectiveLayer = groundLayer == 0 ? ~0 : groundLayer;

        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        float rayDistance = minHeightFromGround + 0.5f;

        // 1. Regular Ground Detection (Looking Down)
        Debug.DrawRay(rayStart, Vector3.down * rayDistance, Color.green);
        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance, effectiveLayer))
        {
            float distance = hit.distance - 0.1f;

            if (distance < minHeightFromGround && throttleInput <= 0.01f)
            {
                // Significantly stronger force if we are sinking
                float error = minHeightFromGround - distance;
                verticalForce += error * 100f; // Increased from 60f
                
                // Kill downward velocity immediately when touching ground
                if (rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                }

                if (rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.7f, rb.linearVelocity.z);
                }
            }

            if (distance < minHeightFromGround * 1.5f && rb.linearVelocity.y < -0.1f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
            }
        }
        else
        {
            // 2. Emergency Recovery (Looking Up)
            // If we don't hit ground below, check if there is ground ABOVE us (we fell through)
            if (Physics.Raycast(transform.position, Vector3.up, out hit, 1.0f, effectiveLayer))
            {
                // We are trapped below a plane! Teleport back up or push hard
                Debug.LogWarning("Drone trapped below plane! Recovering...");
                transform.position = hit.point + Vector3.up * minHeightFromGround;
                rb.linearVelocity = Vector3.zero;
            }
        }
        
        rb.AddForce(Vector3.up * verticalForce, ForceMode.Acceleration);
    }

    private void HandleMovement()
    {
        // Move horizontal/vertical relative to the drone's orientation
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        
        // Add "Landing Friction"
        // If we are touching or very close to the ground and not trying to move
        RaycastHit hit;
        if (moveInput.magnitude < 0.01f && Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, minHeightFromGround + 0.05f, groundLayer == 0 ? ~0 : groundLayer))
        {
            // Dampen horizontal movement to stop sliding on the floor
            Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(-horizontalVel * 5f, ForceMode.Acceleration);
        }

        rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
    }

    private void HandleRotation()
    {
        // Rotate around Y axis (Yaw)
        float yaw = yawInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, yaw, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void HandleTilt()
    {
        // Tilt the drone based on movement input for visual effect and slight physics push
        float targetPitch = moveInput.y * tiltAmount;
        float targetRoll = -moveInput.x * tiltAmount;

        // Smoothly interpolate towards the target tilt
        Quaternion targetRotation = Quaternion.Euler(targetPitch, transform.eulerAngles.y, targetRoll);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime);
    }
}
