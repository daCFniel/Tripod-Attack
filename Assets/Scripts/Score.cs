using TMPro;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class Score : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScoreText(Shooter shooter)
    {
        scoreText.text = shooter.numberOfCells.ToString();
    }
}