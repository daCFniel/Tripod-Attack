using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Delay : MonoBehaviour
{
    public GameObject beginButton;

    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = beginButtonDelay(31f);
        StartCoroutine(coroutine);
    }

    IEnumerator beginButtonDelay(float delay)
    {
        beginButton.SetActive(false);
        yield return new WaitForSeconds(delay);
        beginButton.SetActive(true);
    }
}