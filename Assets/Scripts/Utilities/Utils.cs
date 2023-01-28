using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class Utils : MonoBehaviour
{
    public string firstLevel;
    // Update is called once per frame
    void Start()
    {
        Debug.Log("Game Started.");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting Game...");
        SceneManager.LoadScene(firstLevel);
    }
}
