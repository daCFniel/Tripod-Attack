using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadNotes : Interactable
{
    bool IsFocused = false;
    bool Interacted = false;
    public GameObject note;
    MouseRotation mouseRotation;
    AudioSource noteReadSound;

    private void Start()
    {
        mouseRotation = GameObject.FindGameObjectWithTag("Player").transform.Find("Camera").GetComponent<MouseRotation>();
        noteReadSound = GameObject.Find("Note SFX").GetComponent<AudioSource>();
    }
    public override void OnFocus()
    {
        if (!IsFocused)
        {
            Debug.Log("Looking at: " + gameObject.name);
            IsFocused = true;
        }
    }

    public override void OnInteract()
    {
        if (!Interacted) noteReadSound.Play();
        Interacted = true;
        note.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mouseRotation.canRotate = false;
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stopped looking at: " + gameObject.name);
        IsFocused = false;
    }

    public void ExitNote()
    {
        Interacted = false;
        note.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mouseRotation.canRotate = true;
    }
}