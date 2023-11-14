
using UnityEngine;


// this class manages the player collision
public class CollisionManager : MonoBehaviour
{
    // Create gameObject parent references for each life UI in the game
    public GameObject PlayerLife1;
    public GameObject PlayerLife2;
    public GameObject PlayerLife3;

    // Create references to the different menu's in the game
    public GameObject MainGameUI;
    public GameObject GameOverUI;
    public GameObject CongratsUI;
    

    private int _lives; 
    void Start()
    {
        _lives = 3; // set lives
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if the player collides with the sun, open the congrats screen and disable the other UI
        if (collision.collider.CompareTag("sun"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>()); // Make sure the player mesh does not get effected by the collision
            MainGameUI.SetActive(false);
            CongratsUI.SetActive(true);
        }

        // if the player collides with a sattelite, remove a life and a Life UI image
        if (collision.collider.CompareTag("satt"))
        {
            _lives -= 1;
            
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>()); // Make sure the player mesh does not get effected by the collision
            switch (_lives) // check how many lives are left and remove the appropriate LifeUI accordingly
            {
                case 2:
                    PlayerLife3.SetActive(false);
                    break;
                case 1: PlayerLife2.SetActive(false);
                    break;
                case 0: PlayerLife1.SetActive(false);
                    break;
                
                default:
                    break;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if the player is out of lives
        if (_lives == 0)
        {
            // Go to game over scene
            MainGameUI.SetActive(false);
            GameOverUI.SetActive(true);
            _lives = 4; // Just so that the UI does not keep resetting
        }
    }
}
