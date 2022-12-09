using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadNotes : Interactable
{
    bool IsFocused = false;
    public GameObject noteObject;
    public override void OnFocus()
    {
        if (!IsFocused)
        {
            print("Looking at: " + gameObject.name);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            IsFocused = true;
        }
    }

    public override void OnInteract()
    {
        noteObject.SetActive(true);
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at: " + gameObject.name);
        IsFocused = false;
    }

    public void ExitNote()
    {
        noteObject.SetActive(false);
        Cursor.visible = false;
    }
}