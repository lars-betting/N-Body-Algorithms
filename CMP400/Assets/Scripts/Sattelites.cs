
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// this class is responsible for creating and spawning the sattellites using the sattelite prefab
public class Sattelites : MonoBehaviour
{
    
    // Amount of prefabs to be spawned upon the start of the demo
    public int totalSattelites = 500;
    
    // The maximum radius of the space
    public int maxRadius = 200;
    
    // Array to store the sattelites
    public GameObject[] sattelites;
    
    // A gameObject that represents the sattelite that needs to be copied
    public GameObject satToCopy;
    
    // An array to store the materials that can be applied to the sattelites [at the moment there is just One material used]
    public Material[] materials;
   

    private void Awake()
    {
        sattelites = new GameObject[totalSattelites]; // Initialize array to the correct value
           
    }
    void Start()
    {
        sattelites = CreateSattelites(totalSattelites, maxRadius); // Return the array with the correct data 
        
    }   

  
    // this function creates and spawns the sattelites and stores them in an array
    // It attributes values to each sattelite such as position, mass and material
    // As well as an initial force
    public GameObject[] CreateSattelites(int count, int radius)
    {
        for(int i = 0; i < count; i ++)
        {
            var sp = GameObject.Instantiate(satToCopy); // spawn sattelite
            Vector3 pos = new Vector3(Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius)); // give the sattelite a random position
            
            // the following code was added to ensure the sattelites dont spawn inside the sun
            if (pos.y >= 0)
            {
                pos.y += 20;
            }
            else
            {
                pos.y -= 20;
            }
            if (pos.x >= 0)
            {
                pos.x += 20;
            }
            else
            {
                pos.x -= 20;
            }
            if (pos.z >= 0)
            {
                pos.z += 20;
            }
            else
            {
                pos.z -= 20;
            }
            sp.transform.position = this.transform.position + pos; 
            
            // Scale the sattelites to be various sizes
            sp.transform.localScale *= Random.Range(5f, 10f);
            
            // Give each sattelite a random mass
            sp.GetComponent<Rigidbody>().mass = Random.Range(1, 5);
            
            // Give each sattelite a random material
            sp.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
            
            // Add an initial force to  the sattelite, also happens in GObject.
            sp.transform.GetComponent<Rigidbody>().AddForce(sp.transform.forward, ForceMode.Impulse);

            //Store the sattelite in the array
            sattelites[i] = sp;
        }
        return sattelites;
    }
}
