using UnityEngine;

public class IndustrialMachine : MonoBehaviour
{
   

    private void Start()
    {
   
    }

    private Vector2 moveInput;

    public void SetInputs(Vector2 move,float throttle, float yaw)
    {
        moveInput = move;
    }

    private void Update()
    {
       
        if (moveInput.magnitude > 0.01f) // greater than minimum noise threshold
        {
            Vector3 finalMove = new Vector3(moveInput.x, 0, moveInput.y);
            transform.Translate(finalMove * Time.deltaTime * 2f, Space.World);
        }
    }

   
}
