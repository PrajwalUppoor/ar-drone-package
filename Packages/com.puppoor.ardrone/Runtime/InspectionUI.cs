using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InspectionUI : MonoBehaviour
{
    [Header("Refined Controls")]
    [SerializeField] private TMP_Dropdown targetDropdown; 
    [SerializeField] private Button placeMachineButton;
    [SerializeField] private DroneInput droneInput;
    [SerializeField] private ARPlacement arPlacement;

    [Header("Status UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        if (targetDropdown != null)
            targetDropdown.onValueChanged.AddListener(OnTargetChanged);

        if (placeMachineButton != null)
            placeMachineButton.onClick.AddListener(OnPlaceMachineClicked);
        
        if (droneInput == null) droneInput = FindFirstObjectByType<DroneInput>();
        if (arPlacement == null) arPlacement = FindFirstObjectByType<ARPlacement>();
    }

    private void OnTargetChanged(int index)
    {
        if (droneInput != null)
        {
            droneInput.controllingMachine = (index == 1); // 0 = Drone, 1 = Machine
            string targetName = droneInput.controllingMachine ? "Machine" : "Drone";
            UpdateStatus("Controlling: " + targetName);
        }
    }

    private void OnPlaceMachineClicked()
    {
        if (arPlacement != null)
        {
            arPlacement.PlaceMachineAtCamera();
            UpdateStatus("Machine Placed!");
        }
    }

    public void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}
