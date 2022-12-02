/// </summary>
/// Script for controlling the audio settings of the Alien Tripod Attack game.
/// 
/// @author James Venables
/// @version 1 - 02.12.2022
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Class controlling the audio settings.
public class AudioManager : MonoBehaviour
{
    public AudioMixer theMixer;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("MasterVol"))
        {
            theMixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol"));
        }
        else if (PlayerPrefs.HasKey("MusicVol"))
        {
            theMixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol"));
        }
        else if (PlayerPrefs.HasKey("SFXVol"))
        {
            theMixer.SetFloat("SFXVol", PlayerPrefs.GetFloat("SFXVol"));
        }
    }
}