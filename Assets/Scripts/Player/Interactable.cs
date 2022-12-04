using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    // Interactable layer ID in the insperctor
    public const int interactableLayerId = 6;
    public virtual void Awake()
    {
        // Any object that implements Interactable will be automatically given an interactable layer
        gameObject.layer = interactableLayerId; 
    }
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();

}
