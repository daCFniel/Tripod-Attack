using UnityEngine;

public class FallenTree : Interactable
{
    bool IsFocused = false;
    private Inventory inventory;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").transform.Find("Inventory").GetComponent<Inventory>();
    }

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

        if (inventory.hasAxe)
        {
            Destroy(this.gameObject);
        }
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at: " + gameObject.name);
        IsFocused = false;
    }
}
