/// </summary>
/// Script for controlling the options menu of the Alien Tripod Attack game.
/// 
/// @author James Venables
/// @version 1 - 30.11.2022
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Class controlling the options screen.
public class OptionsScreen : MonoBehaviour
{
    public Toggle fullScreenTog;
    public Toggle vsyncTog;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;

    public TMP_Text resolutionLabel;

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
}
// Class controlling the resolution settings.
[System.Serializable]
public class ResItem
{
    public int horizontal;
    public int vertical;


}