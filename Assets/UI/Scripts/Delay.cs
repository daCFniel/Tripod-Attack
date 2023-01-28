using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Delay : MonoBehaviour
{
    public GameObject beginButton;

    private IEnumerator coroutine;

    public float seconds;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = beginButtonDelay(seconds);
        StartCoroutine(coroutine);
    }

    IEnumerator beginButtonDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        beginButton.SetActive(true);
    }
}