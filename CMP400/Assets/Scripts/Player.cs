
using UnityEngine;

// This class handles player input 
public class Player : MonoBehaviour
{
    // set player speed
    public float speed = 1f;
    
    // Values to increment yaw and roll with
    public float addYaw = 100f;
    public float addRoll = 100f;
    
    // create yaw value
    private float yaw;
    

    // Update is called once per frame
    void Update()
    {
        
        //Set the input for wasd keys (or arrow keys)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate Yaw 
        yaw += horizontalInput * addYaw * Time.deltaTime;
        
        // Calcuting pitch and role rotation to a certain angle between a minimum and maximum angle
        float pitch = Mathf.Lerp(0f, 20f, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
        float roll = Mathf.Lerp(0f, 30f, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput);
        
        // moves the object in the direction that it is facing
        transform.position +=  speed * Time.deltaTime * transform.forward;

        // rotates the object
        transform.Rotate(verticalInput * addYaw * Time.deltaTime, 0f, -horizontalInput * addRoll * Time.deltaTime);



    }
}
