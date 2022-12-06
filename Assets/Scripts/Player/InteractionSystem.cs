using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class InteractionSystem : MonoBehaviour
{
    [Header("Interactions")]
    [SerializeField] bool canInteract = true;
    [SerializeField] KeyCode interactKey = KeyCode.E;
    [SerializeField] Vector3 interactionRayPoint = default;
    [SerializeField] float interactionDistance = default;
    [SerializeField] LayerMask interactionLayer = default;
    Interactable currentInteractable;
    Camera cameraComponent;
    bool ShouldInteract => Input.GetKeyDown(interactKey) && currentInteractable != null;

    private void Start()
    {
        cameraComponent = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            ProcessInteractions();
            HandleInteractionsInput();
        }
    }

    // Perform an action after pressing an interaction key
    private void HandleInteractionsInput()
    {
        if (ShouldInteract && Physics.Raycast(cameraComponent.ViewportPointToRay(interactionRayPoint), out _, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }

    // Look for interactable objects
    private void ProcessInteractions()
    {
        // Take into consideration all colliders when checking for interactable objects (To prevent interacting through objects)
        if (Physics.Raycast(cameraComponent.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            bool collidedWithInteractableLayer = hit.collider.gameObject.layer == Interactable.interactableLayerId;
            // If its on Interactable layer and the character is not currently interacting (or is changing focus to other object next by)
            if (collidedWithInteractableLayer && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                // Try to get the object the ray collided with
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        } // Stop interacting
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }
}
