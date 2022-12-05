using System.Collections;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Feature")]
    [SerializeField] bool canZoom = true;
    [SerializeField] KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] float timeToZoom = 0.4f;
    [SerializeField] float zoomFOV = 30f; // Target camera Field of View when zooming in
    float defaultFOV;
    Coroutine zoomRoutine;
    Camera cameraComponent;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
        defaultFOV = cameraComponent.fieldOfView;
    }


    // Update is called once per frame
    void Update()
    {
        if (canZoom) ZoomCamera();
    }


    private void ZoomCamera()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            // Start zooming in
            zoomRoutine = StartCoroutine(ToggleCameraZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            // Start zooming out
            zoomRoutine = StartCoroutine(ToggleCameraZoom(false));
        }
    }


    private IEnumerator ToggleCameraZoom(bool isZoomed)
    {
        float targetFOV = isZoomed ? zoomFOV : defaultFOV;
        float startingFOV = cameraComponent.fieldOfView;
        float timeElapsed = 0f;

        while (timeElapsed < timeToZoom)
        {
            cameraComponent.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cameraComponent.fieldOfView = targetFOV;
        zoomRoutine = null;
    }

}