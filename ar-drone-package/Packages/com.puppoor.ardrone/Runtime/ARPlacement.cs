using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject machinePrefab;
    
    private GameObject spawnedDrone;
    private GameObject spawnedMachine;
    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Ignore UI touches
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    // Place Drone if not already there
                    if (spawnedDrone == null)
                    {
                        spawnedDrone = Instantiate(dronePrefab, hitPose.position, hitPose.rotation);
                    }
                    else if (spawnedMachine == null)
                    {
                        // Allow moving drone if machine hasn't been placed yet
                        spawnedDrone.transform.position = hitPose.position;
                    }
                }
            }
        }
    }

    public void PlaceMachineAtCamera()
    {
        if (machinePrefab == null) return;

        // Place 1.5 meters in front of the camera
        Transform camTransform = Camera.main.transform;
        Vector3 spawnPos = camTransform.position + camTransform.forward * 1.5f;
        
        // Align to ground if possible
        RaycastHit hit;
        if (Physics.Raycast(spawnPos + Vector3.up, Vector3.down, out hit, 10f))
        {
            spawnPos = hit.point;
        }

        if (spawnedMachine == null)
        {
            spawnedMachine = Instantiate(machinePrefab, spawnPos, Quaternion.identity);
            
            // Re-find the machine in DroneInput if it was just spawned
            DroneInput di = FindFirstObjectByType<DroneInput>();
            if (di != null) di.SetMachineTarget(spawnedMachine.GetComponent<IndustrialMachine>());
        }
        else
        {
            spawnedMachine.transform.position = spawnPos;
        }

        Debug.Log("Machine placed via button at " + spawnPos);
    }
}
