using UnityEngine;
using System.Collections;
using TMPro;

public class TextStyle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;

    public string[] stringArray;

    [SerializeField] float timeBtwnChars;
    [SerializeField] float timeBtwnWords;

    int i = 0;

    private void Start()
    {
        EndCheck();
    }

    void EndCheck()
    {
        if (i <= stringArray.Length - 1)
        {
            _textMeshPro.text = stringArray[i];
            StartCoroutine(TextVisible());
        }
    }

    private IEnumerator TextVisible()
    {
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;

            if(visibleCount >= totalVisibleCharacters)
            {
                i += 1;
                Invoke("EndCheck", timeBtwnChars);
                break;
            }    

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }
    }
}