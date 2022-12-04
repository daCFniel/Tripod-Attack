using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class TestInteractable : Interactable
{
    public override void OnFocus()
    {
        print("Looking at: " + gameObject.name);
    }

    public override void OnInteract()
    {
        print("Interacted with: " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at: " + gameObject.name);
    }
}
