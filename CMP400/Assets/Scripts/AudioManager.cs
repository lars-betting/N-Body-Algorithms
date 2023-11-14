
using UnityEngine;

//handles the audio in the game
public class AudioManager : MonoBehaviour
{
   // reference the audio source
   public AudioSource Audio;

   private void Awake()
   {
      //Play the audio file
      Audio.Play();
      DontDestroyOnLoad(gameObject);
   }
}
