using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

[CreateAssetMenu(menuName ="Inventory Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public string description;
}
