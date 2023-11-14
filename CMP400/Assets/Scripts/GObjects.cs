
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// This script is attached to each object in the scene that is part of the gravity simulation
public class GObjects : MonoBehaviour
{
    public Rigidbody rb; // Get reference to rigidbody to speed up calculations
    
    // Set variables for initial impulse and the applied force
    public Vector3 initialForce;  
    public Vector3 appliedForce;

    private void Awake()
    {
        // initialize the rigidbody reference and set the components we will use to zero to avoid {nan}
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        appliedForce = Vector3.zero;
       
    }
    void Start()
    {
        // give the object a random force to when it spawns
        initialForce = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
        rb.AddForce(initialForce * rb.mass, ForceMode.Impulse);
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GCalcBruteForce.gObjects.Add(this); // add this object to the list
        }
        else
        {
           GCalc.gObjects.Add(this); // add this object to the list
        }
    }

}
