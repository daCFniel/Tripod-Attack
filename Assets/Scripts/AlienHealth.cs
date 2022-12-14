using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class AlienHealth : MonoBehaviour
{
    private float health = 3;
    [SerializeField]
    private AudioSource alienScreech;
    [SerializeField]
    private AudioSource alienGrowling;
    public GameObject smoke, steam, flare;
    public void ReduceHealth()
    {
        if (health > 0)
        {
            health--;
            DamageAlien();
        }
            
    }
    // Update is called once per frame
    private bool AlienHasZeroHealth()
    {
        return health == 0;
    }

    private void DamageAlien()
    {
        if (health == 2)
        {
            steam.SetActive(true);
        } else if (health == 1)
        {
            smoke.SetActive(true);
        }
        else if (AlienHasZeroHealth()) DestroyAlien();
    }

    private void DestroyAlien()
    {
        flare.SetActive(true);
        alienGrowling.Play();
        alienScreech.PlayDelayed(5.0f);
        StartCoroutine(loadMainLevel());
    }

    IEnumerator loadMainLevel()
    {
        yield return new WaitForSeconds(10.0f);
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayScreechAudio() {
        if (!alienScreech.isPlaying) {
            alienScreech.Play();
        }
    }
}
