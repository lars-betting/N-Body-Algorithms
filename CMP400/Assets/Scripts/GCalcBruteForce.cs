
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;  

public class GCalcBruteForce : MonoBehaviour
{
    // create a list that keeps track of all objects in the scene
    public static List<GObjects> gObjects;

    [Range(-1, 20)]
    public float G = 1.0f; // gravity constant, I've added -1 to show a potential reverse gravity effect (elements being pushed away from the source)
    
    [Range(0.01f, 5f)]
    public float timeScale = 1f; //Speed of the simulation
    
    public float deltaTime; // For measuring fps
    public Text fpsText; // UI text for showing current fps
    public Text IfpsText; // UI text for showing Intended fps (what fps is the minimum we aim for)
    

    private void Start()
    {
        gObjects = new List<GObjects>(); // initialize list 
    }
    
    
    void CalcBruteForce()
    {
        foreach (GObjects obj in gObjects)
        {
            
            obj.appliedForce = Vector3.zero; // Set appliedForce to zero so that the force of one object does not carry over to the next
            foreach (GObjects obj2 in gObjects)
            {
                if (obj != obj2) // make sure that the objects are not calculating the force it exerts on itself
                {
                    CalcForce(obj, obj2); // Calculate the force between the two objects
                }
            }
        }
    }

    // The Semi-Implicit Euler Method, this method calculates the position and velocity of each object
    // It does so by first calculating the velocity for the next timestep using the acceleration of the object, and then using 
    // this value to calculated the new position. It then resets the applied force to zero so that the values are not 
    // carried over for the next object.
    void SIEuler(GObjects obj)
    {
        
        Vector3 acceleration = obj.appliedForce / obj.rb.mass;
            obj.rb.velocity += acceleration * Time.fixedDeltaTime;
            obj.rb.position += obj.rb.velocity * Time.fixedDeltaTime;
            obj.appliedForce = Vector3.zero;
    }
    
    // This demo uses fixedUpdate instead of update to make sure that each frame happens at a stable time interval for accurat
    // gravity calculations
    void FixedUpdate()
    {
     //call this once per frame 50fps
     
     CalcBruteForce();
    
     // Apply the semi implicit euler integrator
     foreach (GObjects obj in gObjects)
     {
           SIEuler(obj);
           
     }
     
     // Set intended fps equal to value chosen for fixed Delta Time
     IfpsText.text = Mathf.Ceil(1.0f/Time.fixedDeltaTime).ToString();
     Time.timeScale = timeScale; 
       
    }
    
    void CalcForce(GObjects b1, GObjects b2)
    {
        // Same as the function above, instead we use the mass and position of the object
        Vector3 direction = b1.transform.position - b2.transform.position;
        float distance = direction.magnitude;
        
        Vector3 force = G * (b1.rb.mass * b2.rb.mass / Mathf.Pow(distance, 2)) * direction.normalized;
        b1.appliedForce -= force;
    }
    
    // Update is called once per frame
        void Update()
        {
            // calculate the actual delta time for UI
            deltaTime += (Time.deltaTime - deltaTime);
           float fps = 1.0f / deltaTime;
           fpsText.text = Mathf.Ceil(fps).ToString();
        }
}

