/// </summary>
/// Script for controlling the options menu of the Alien Tripod Attack game.
/// 
/// @author James Venables
/// @version 1 - 02.12.2022
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

// Class controlling the options screen.
public class OptionsScreen : MonoBehaviour
{
    public Toggle fullScreenTog;
    public Toggle vsyncTog;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;

    public TMP_Text resolutionLabel;

    public AudioMixer theMixer;

    public TMP_Text mastLabel;
    public TMP_Text musicLabel;
    public TMP_Text sfxLabel;

    public Slider mastSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        fullScreenTog.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            vsyncTog.isOn = false;
        } else
        {
            vsyncTog.isOn = true;
        }

        bool foundRes = false;
        for(int i = 0; i < resolutions.Count; i++)
        {
            if(Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundRes = true;

                selectedResolution = i;

                updateResLabel();
            }
        }

        if(!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            selectedResolution = resolutions.Count - 1;

            updateResLabel();
        }

        // For removing the default starting values in the audio settings:
        float vol = 0f;
        theMixer.GetFloat("MasterVol", out vol);
        mastSlider.value = vol;
        theMixer.GetFloat("MusicVol", out vol);
        musicSlider.value = vol;
        theMixer.GetFloat("SFXVol", out vol);
        sfxSlider.value = vol;

        mastLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString();
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Function for switching to the left resolution option.
    public void ResLeft()
    {
        selectedResolution--;

        if(selectedResolution < 0)
        {
            selectedResolution = 0;
        }

        updateResLabel();
    }

    // Function for switching to the right resolution option.
    public void ResRight()
    {
        selectedResolution++;

        if(selectedResolution > resolutions.Count - 1)
        {
            selectedResolution = resolutions.Count - 1;
        }

        updateResLabel();
    }

    public void updateResLabel()
    {
        resolutionLabel.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    // Function for the 'APPLY GRAPHICS CHANGES' button
    public void applyGraphics()
    {
        //Screen.fullScreen = fullScreenTog.isOn;

        if(vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        } else
        {
            QualitySettings.vSyncCount = 0;
        }

        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullScreenTog.isOn);
    }

    // Function for controling the master volume control setting.
    public void setMasterVolume()
    {
        mastLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString();

        theMixer.SetFloat("MasterVol", mastSlider.value);

        PlayerPrefs.SetFloat("MasterVol", mastSlider.value);    // used to remember user's previous settings/selections
    }

    // Function for controling the music volume control setting.
    public void setMusicVolume()
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();

        theMixer.SetFloat("MusicVol", musicSlider.value);

        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);    // used to remember user's previous settings/selections
    }

    // Function for controling the SFX volume control setting.
    public void setSFXVolume()
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();

        theMixer.SetFloat("SFXVol", sfxSlider.value);

        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);    // used to remember user's previous settings/selections
    }
}
// Class controlling the resolution settings.
[System.Serializable]
public class ResItem
{
    public int horizontal;
    public int vertical;
}