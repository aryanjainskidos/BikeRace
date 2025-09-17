using UnityEngine;

public class ApplicationExit : MonoBehaviour
{
    // Call this method when you want to quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game called!");
        Application.Quit();

        // For testing inside the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}