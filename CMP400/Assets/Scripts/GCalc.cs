
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;

struct BarnesHut
{
    public int nrOfBodies; // Amount of bodies inside the current Node
    public Vector3 centerOfMass; // Center of mass of current node
    public float mass; // Total Mass of the current node
    public Vector3 position; // Position of the node relative to the center of the space
    public Vector3 size; // Size of current node
    public BarnesHut[] bh; // Array of children (Will be set to 8 later in this script)
    public List<GObjects> objectsInChild; // List of objects inside this node
};

public class GCalc : MonoBehaviour
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
    
    public Vector3 rootSize = new Vector3(400, 400, 400); // Size for total space


    public float theta = 0.5f; // value for theta, for deciding the accuracy of the simulation

    private void Start()
    {
        gObjects = new List<GObjects>(); // initialize list 
    }

  
    // This function calculates the gravity for a single obj through traversing the Tree
    // It takes as parameters, the current node, the obj that we use for force calculation, 
    // and the value theta to set our accuracy for the simulation
    
    void calcTreeCode(BarnesHut bh, GObjects thisObj, float theta) 
    {
        if (thisObj.transform.position != bh.centerOfMass) // check if the object is not checking the force it exerts on itself
        {
            var d = Vector3.Distance(bh.centerOfMass, thisObj.transform.position); // Distance of the current node
            var s = bh.size.magnitude; // Calculate the magnitude of the size of the current node
            float division = theta * s / d; // Check if value is smaller than theta
            if (bh.bh != null) // Make sure the current node has children ( Should only happen in the case of an error or bug)
            {
                if (division < theta) 
                {
                    // use the mass and center of mass of the node to represent all bodies within the node
                    CalcBHForce(thisObj, bh.mass, bh.centerOfMass);
                }
                else // traverse further
                {
                    if (bh.nrOfBodies == 1) // if the current node only contains one body
                    {
                        // Can not traverse this node further
                        // bh.mass and bh.centerOfMass are actually the mass and position of the only object in the node.
                        CalcBHForce(thisObj, bh.mass, bh.centerOfMass);
                    }
                    else
                    {
                        // For each of the node's children, repeat the steps above recursively
                        foreach (BarnesHut childBH in bh.bh)
                        {
                            if (childBH.nrOfBodies > 0) 
                            {
                                // Only repeat the steps if this child has at least one body
                                calcTreeCode(childBH, thisObj, theta);
                            }
                        }
                    }

                }
            }
            else // if the node has no children, use the current node to calculate the forces on the object (in case of Bug or error)
            {
                CalcBHForce(thisObj, bh.mass, bh.centerOfMass);
            }
            
        }
    }
    
    // This function checks whether the current body is inside a node 
    bool IsInBH(BarnesHut bh, Vector3 position) 
    {
        // Get the beginning and end of the node
        Vector3 childBegin = bh.position - (bh.size / 2); 
        Vector3 childEnd = bh.position + (bh.size / 2);
        
        // check if the objects x, y and z coordinates are inside the node
        if (position.x >= childBegin.x && position.x <= childEnd.x &&
            position.y >= childBegin.y && position.y <= childEnd.y &&
            position.z >= childBegin.z && position.z <= childEnd.z)
        {
            return true; // Object is inside the node
        }

        return false; // Object is outside the node
    }

    // This function creates the Tree Data Structure
    // It takes in the following parameters:
    // Level: This determines the maximum depth of the tree (default set to 100 for maximum depth)
    // NewPosition: The position of the current node, this is used to set the right position for it's children
    // objectsList: This is the list of objects that was stored in the parent node. 
    // This optimizes the tree by it not having to constantly 
    // Search through the entire list of bodies for each node, making it quicker to calculate the mass,
    // centerOfMass and NrOfBodies for each node
    // NewSize: The Size of the current node, this is used to set the size for it's children 
    
    BarnesHut CreateBHTree(int level, Vector3 NewPosition, List<GObjects> objectsList, Vector3 newSize)
    {
        // Create the current Node 
        BarnesHut currentBH = new BarnesHut
        {
            // set the position, size and the objectsInChild list based on the values provided by the parent node
            position = NewPosition,
            size = newSize,
            objectsInChild = new List<GObjects>()
        }; 

        foreach (GObjects obj in objectsList)
        {
            // for each of the objects that are inside this node, calculate the corresponding values for the node
            if (IsInBH(currentBH, obj.transform.position))
            {
                currentBH.nrOfBodies += 1;
                currentBH.mass += obj.rb.mass;
                currentBH.centerOfMass += obj.rb.mass * obj.transform.position; 
                currentBH.objectsInChild.Add(obj);
            }
        }
        // calculates the centerOfMass
        if (currentBH.nrOfBodies > 0)
        {
            currentBH.centerOfMass /= currentBH.mass;
        }

        // if maximum depth has not been reached, and the node has more than one body
        // Then create it's children using the values of the current node
        
        if (level > 0 && currentBH.nrOfBodies > 1)
        {
            currentBH.bh = new BarnesHut[8]; // initialize children

            // Divide the size of the node by two to get the corresponding size for each child
            // Because size is a 3D vector, each component gets divided by two, so technically we obtain an eighth of 
            // the current node
            
            Vector3 halfChildSize = currentBH.size / 2;
            
            
            // The following code calculates the new position for each of the nodes children. 

            currentBH.bh[0].position.x = currentBH.position.x - (currentBH.size.x / 2) + (currentBH.size.x / 4);
            currentBH.bh[0].position.y = currentBH.position.y + (currentBH.size.y / 2) - (currentBH.size.y / 4);
            currentBH.bh[0].position.z = currentBH.position.z + (currentBH.size.z / 2) - (currentBH.size.z / 4);
            
            // Create the child node and recursively it's children using the calculated values.
            currentBH.bh[0] = CreateBHTree(level - 1, currentBH.bh[0].position, currentBH.objectsInChild, halfChildSize);
            
            //1
            currentBH.bh[1].position.x = currentBH.position.x + (currentBH.size.x / 2) - (currentBH.size.x / 4);
            currentBH.bh[1].position.y = currentBH.position.y + (currentBH.size.y / 2) - (currentBH.size.y / 4);
            currentBH.bh[1].position.z = currentBH.position.z + (currentBH.size.z / 2) - (currentBH.size.z / 4);
    
            currentBH.bh[1] = CreateBHTree(level - 1, currentBH.bh[1].position, currentBH.objectsInChild, halfChildSize);
   
    
            //2 
            currentBH.bh[2].position.x = currentBH.position.x + (currentBH.size.x / 2) - (currentBH.size.x / 4);
            currentBH.bh[2].position.y = currentBH.position.y + (currentBH.size.y / 2) - (currentBH.size.y / 4);
            currentBH.bh[2].position.z = currentBH.position.z - (currentBH.size.z / 2) + (currentBH.size.z / 4);
    
            currentBH.bh[2] = CreateBHTree(level - 1, currentBH.bh[2].position, currentBH.objectsInChild, halfChildSize);
           
    
            //3 
            currentBH.bh[3].position.x = currentBH.position.x - (currentBH.size.x / 2) + (currentBH.size.x / 4);
            currentBH.bh[3].position.y = currentBH.position.y + (currentBH.size.y / 2) - (currentBH.size.y / 4);
            currentBH.bh[3].position.z = currentBH.position.z - (currentBH.size.z / 2) + (currentBH.size.z / 4);
    
            currentBH.bh[3] = CreateBHTree(level - 1, currentBH.bh[3].position, currentBH.objectsInChild, halfChildSize);
            
    
            // 4
            currentBH.bh[4].position.x = currentBH.position.x - (currentBH.size.x / 2) + (currentBH.size.x / 4);
            currentBH.bh[4].position.y = currentBH.position.y - (currentBH.size.y / 2) + (currentBH.size.y / 4);
            currentBH.bh[4].position.z = currentBH.position.z + (currentBH.size.z / 2) - (currentBH.size.z / 4);
    
            //childBH.bh[4].size = halfChildSize;
    
            currentBH.bh[4] = CreateBHTree(level - 1, currentBH.bh[4].position, currentBH.objectsInChild, halfChildSize);
          
            //5
            currentBH.bh[5].position.x = currentBH.position.x + (currentBH.size.x / 2) - (currentBH.size.x / 4);
            currentBH.bh[5].position.y = currentBH.position.y - (currentBH.size.y / 2) + (currentBH.size.y / 4);
            currentBH.bh[5].position.z = currentBH.position.z + (currentBH.size.z / 2) - (currentBH.size.z / 4);
    
            currentBH.bh[5] = CreateBHTree(level - 1, currentBH.bh[5].position, currentBH.objectsInChild, halfChildSize);
            
    
            //6 
            currentBH.bh[6].position.x = currentBH.position.x + (currentBH.size.x / 2) - (currentBH.size.x / 4);
            currentBH.bh[6].position.y = currentBH.position.y - (currentBH.size.y / 2) + (currentBH.size.y / 4);
            currentBH.bh[6].position.z = currentBH.position.z - (currentBH.size.z / 2) + (currentBH.size.z / 4);
    
           
            currentBH.bh[6] = CreateBHTree(level - 1, currentBH.bh[6].position, currentBH.objectsInChild, halfChildSize);
            //7 
            currentBH.bh[7].position.x = currentBH.position.x - (currentBH.size.x / 2) + (currentBH.size.x / 4);
            currentBH.bh[7].position.y = currentBH.position.y - (currentBH.size.y / 2) + (currentBH.size.y / 4);
            currentBH.bh[7].position.z = currentBH.position.z - (currentBH.size.z / 2) + (currentBH.size.z / 4);
    
             currentBH.bh[7] = CreateBHTree(level - 1, currentBH.bh[7].position, currentBH.objectsInChild, halfChildSize);
            
        }

        // return the current node
        return currentBH;
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
     BarnesHut root = CreateBHTree(100, new Vector3(0.0f, 0.0f,0.0f), gObjects, rootSize);
     
     //calculate tree code
     foreach (GObjects obj in gObjects)
     {
         calcTreeCode(root, obj, theta);
     }
     
     // Apply the semi implicit euler integrator
     foreach (GObjects obj in gObjects)
     {
           SIEuler(obj);
           
     }
     
     // Set intended fps equal to value chosen for fixed Delta Time
     IfpsText.text = Mathf.Ceil(1.0f/Time.fixedDeltaTime).ToString();
     Time.timeScale = timeScale; 
       
    }

    // This function calculates the force on an object by using the center of mass of the current node.
    void CalcBHForce(GObjects obj, float mass, Vector3 position)
    {
        Vector3 direction = obj.transform.position - position; // create vector using both positions
        float distance = direction.magnitude; // get distance through calculating the magnitude of this vector

        // Calculate the force using : F = G * ((m1 * m2) / d^2))
        // Multiply by normalized vector to get the correct direction of the force
        Vector3 force = G * obj.rb.mass * mass / Mathf.Pow(distance, 2) * direction.normalized;
        obj.appliedForce -= force; 
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

