
using UnityEngine;
using UnityEngine.SceneManagement;

// Class is similar to main menu UI but for restarting the game
public class RestartGame : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0); // loads the main menu scene
    }
}
