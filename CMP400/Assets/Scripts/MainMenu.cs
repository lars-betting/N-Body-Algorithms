
using UnityEngine;
using UnityEngine.SceneManagement;

// This class is created to support the UI in the main menu
public class MainMenu : MonoBehaviour
{
    private bool _barnesHut = true; // bool to check which game mode is being played
    public void SetGameMode()
    {
        _barnesHut = false; //Set the game mode to be brute force
    }
    public void PlayGame()
    {
        if (_barnesHut == false)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // load next scene in the queue (Brute Force)
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2); // load the last scene (Barnes Hut)
        }
    }
    
    
   
}
