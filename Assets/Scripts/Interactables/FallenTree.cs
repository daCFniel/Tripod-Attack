using UnityEngine;

public class FallenTree : Interactable
{
    bool IsFocused = false;

    public override void OnFocus()
    {
        if(!IsFocused)
        {
            print("Looking at: " + gameObject.name);
            IsFocused = true;
        }
        
    }

    public override void OnInteract()
    {
        print("Interacted with: " + gameObject.name);

    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at: " + gameObject.name);
        IsFocused = false;
    }
}
