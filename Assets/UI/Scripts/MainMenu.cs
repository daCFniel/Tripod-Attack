/// </summary>
/// Script for controlling the main menu of the Alien Tripod Attack game.
/// 
/// @author James Venables
/// @version 1 - 30.11.2022
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;
    public GameObject optionsScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function for starting the game via the 'START' button
    public void startGame()
    {
        SceneManager.LoadScene(firstLevel);
    }

    // Function for opening the options via the 'OPTIONS' button
    public void openOptions()
    {
        optionsScreen.SetActive(true);
    }

    //  Function for closing the options screen after clicking the 'OPTIONS' button
    public void closeOptions()
    {
        optionsScreen.SetActive(false);
    }

    // Function for quiting the game after clicking the 'QUIT' button
    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
