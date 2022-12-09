using System;
using UnityEngine;

public class GenericItem : Interactable
{
    public static Action<ItemData> OnCollect;
    [SerializeField] ItemData itemData;
    bool IsFocused = false;
    public override void OnFocus()
    {
        if (!IsFocused)
        {
            print("Looking at: " + gameObject.name);
            IsFocused = true;
        }
    }

    public override void OnInteract()
    {
        OnCollect?.Invoke(itemData);
        Destroy(gameObject);
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at: " + gameObject.name);
        IsFocused = false;
    }
}