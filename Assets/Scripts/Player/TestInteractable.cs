using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class TestInteractable : Interactable
{
    public override void OnFocus()
    {
        Debug.Log("Looking at: " + gameObject.name);
    }

    public override void OnInteract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stopped looking at: " + gameObject.name);
    }
}
