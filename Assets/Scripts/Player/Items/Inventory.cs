using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// Attachable to an Inventory game object which is a child of a Player object
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject itemInfoPanel;
    [SerializeField] TextMeshProUGUI itemInfoText = default;
    [SerializeField] Image itemInfoImage = default;
    [SerializeField] int displayInfoTime = 5;

    [Header("Control Flags")]
    public bool hasFlashlight;
    public bool hasBinoculars;
    public bool hasAxe;
    public bool hasGasMask;
    public bool hasScubaMask;

    [Header("Controls")]
    [SerializeField] KeyCode flashlightKey = KeyCode.F;

    [Header("Item Game Objects")]
    [SerializeField] GameObject flashlight;

    [Header("SFX")]
    [SerializeField] AudioSource pickupSound;
    [SerializeField] AudioSource finalMusic;
    [SerializeField] AudioSource ambientMusic;
    [SerializeField] List<AudioClip> ambientTracks;

    [Header("Special Interactions")]
    [SerializeField] GameObject pondTrap;


    // Control lambdas for item effects
    bool ShouldUseFlashlight => Input.GetKeyDown(flashlightKey) && hasFlashlight;

    // List for storing all the items in the player's inventory
    public List<InventoryItem> inventory;

    private void Awake()
    {
        inventory = new List<InventoryItem>();
    }
    private void OnEnable()
    {
        TestItem.OnCollect += Add;
        GenericItem.OnCollect += Add;
    }
    private void OnDisable()
    {
        TestItem.OnCollect -= Add;
        GenericItem.OnCollect -= Add;
    }

    private void Update()
    {
        HandleItems();
    }

    private void HandleItems()
    {
        if (ShouldUseFlashlight)
        {
            flashlight.GetComponent<Flashlight>().ToggleFlashlight();
        }
    }

    public void Add(ItemData itemData)
    {
        pickupSound.Play();
        InventoryItem newItem = new(itemData);
        inventory.Add(newItem);
        Debug.Log($"Added {itemData.displayName} to the Inventory.");
        // Display information about the item
        itemInfoImage.sprite = itemData.icon;
        itemInfoText.text = $"You just picked up the {itemData.displayName}. \n {itemData.description}";
        itemInfoPanel.SetActive(true);
        StartCoroutine(HideItemInfoDialog());
        HandleInventory(itemData);
    }

    private void HandleInventory(ItemData itemData)
    {
        switch (itemData.id)
        {
            case "1": // Flashlight
                hasFlashlight = true;
                break;
            case "2": // Binoculars
                hasBinoculars = true;
                break;
            case "3": // Axe
                hasAxe = true;
                pondTrap.SetActive(true);
                break;
            case "4": // Gas Mask
                hasGasMask = true;
                finalMusic.Play();
                break;
            case "5": // Scuba Mask
                hasScubaMask = true;
                break;
            default:
                Debug.Log("Item ID:" + itemData.id + " not recognized");
                break;
        }
        PlayRandomAmbientTrack();
    }

    private void PlayRandomAmbientTrack()
    {
        AudioClip clip = ambientTracks[UnityEngine.Random.Range(0, ambientTracks.Count)];
        ambientMusic.clip = clip;
        if (!ambientMusic.isPlaying) ambientMusic.Play();
    }

    public void Remove(ItemData itemData)
    {
        InventoryItem item = new(itemData);
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

    private IEnumerator HideItemInfoDialog()
    {
        yield return new WaitForSeconds(displayInfoTime);
        itemInfoPanel.SetActive(false);
    }
}
