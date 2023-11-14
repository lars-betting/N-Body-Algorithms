
using UnityEngine;

// This class handles collision for the Sun
public class SunCollision : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        // if the sun collides with the asteroid, allow the asteroid to pass through
        if (collision.collider.CompareTag("satt"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
