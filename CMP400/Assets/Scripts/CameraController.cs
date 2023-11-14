
using UnityEngine;
// this script controlls the camera position relative to the player
public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;        //Public variable to store a reference to the player game object
    public float rotationSpeed;       // decides how quick the camera rotates based on mouse value 
    private Vector3 offset;            //Private variable to store the offset distance between the player and camera

    // variables for storing the rotation values in 2D
    private float xRotation;   
    private float yRotation;
    
    
    // Use this for initialization
    void Start () 
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }
    
    void Update ()
    {
        // get the mouse coordinates as coordinates between -1 and 1 and multiply by rotation speed
        
        xRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        yRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;

        yRotation = Mathf.Clamp(yRotation, -90f, 90f); // stops the camera from going upside down

        // create a quaternion that stores the rotation values
        Quaternion cameraRotation = Quaternion.Euler(yRotation, xRotation, 0);
        
        //Set the position of the camera to be behind the player relative to where the player is facing
        transform.position = player.transform.position + cameraRotation * offset;
        
        //always have the camera look at the player
        transform.LookAt(player.transform.position);
    }
}
