using UnityEngine;

public class IndustrialMachine : MonoBehaviour
{
    // [Header("Digital Twin Data")]
    // public float temperature = 25f;
    // public float thresholdTemperature = 80f;
    
    // [Header("Visuals")]
    // [SerializeField] private Renderer mainRenderer;
    // [SerializeField] private Color normalColor = Color.white;
    // [SerializeField] private Color overheatColor = Color.red;

    // private Material instancedMaterial;

    private void Start()
    {
        // if (mainRenderer != null)
        // {
        //     instancedMaterial = mainRenderer.material;
        //     instancedMaterial.color = normalColor;
        // }
    }

    private Vector2 moveInput;

    public void SetInputs(Vector2 move,float throttle, float yaw)
    {
        moveInput = move;
    }

    private void Update()
    {
        // Simulate fluctuating temperature
        // temperature += Mathf.Sin(Time.time) * 0.1f;

        // // Visual feedback based on data
        // if (instancedMaterial != null)
        // {
        //     float t = Mathf.InverseLerp(25f, thresholdTemperature, temperature);
        //     instancedMaterial.color = Color.Lerp(normalColor, overheatColor, t);
        // }

        // Handle Movement if we are the current control target
        if (moveInput.magnitude > 0.01f) // greater than minimum noise threshold
        {
            Vector3 finalMove = new Vector3(moveInput.x, 0, moveInput.y);
            transform.Translate(finalMove * Time.deltaTime * 2f, Space.World);
        }
    }

    // public string GetStatusReport()
    // {
    //     return $"Temp: {temperature:F1}°C | Status: {(temperature > thresholdTemperature ? "CRITICAL" : "OK")}";
    // }
}

