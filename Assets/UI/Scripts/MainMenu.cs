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
    public GameObject loadingScreen;
    public GameObject optionsScreen;
    public GameObject creditsScreen;

    public void startGame()
    {
        loadingScreen.SetActive(true);
        Debug.Log("Loading Game...");
    }

    // Function for starting the game via the 'BEGIN...' button
    public void loadGame()
    {
        SceneManager.LoadScene(firstLevel);
        Debug.Log("Starting Game...");
    }

    // Function for opening the options via the 'OPTIONS' button
    public void openOptions()
    {
        optionsScreen.SetActive(true);
        Debug.Log("Opening Options...");
    }

    //  Function for closing the options screen after clicking the 'OPTIONS' button
    public void closeOptions()
    {
        optionsScreen.SetActive(false);
        Debug.Log("Closing Options...");
    }

    // Function for opening the credits screen via the 'CREDITS' button
    public void openCredits()
    {
        creditsScreen.SetActive(true);
        Debug.Log("Opening Credits...");
    }

    // Function for closing the options screen after clicking the 'OPTIONS' button
    public void closeCredits()
    {
        creditsScreen.SetActive(false);
        Debug.Log("Closing Credits...");
    }

    // Function for quiting the game after clicking the 'QUIT' button
    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}