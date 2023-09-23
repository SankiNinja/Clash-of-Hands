using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    [SerializeField]
    private TextMeshProUGUI _currentScoreText;

    public void SetGameOver(int currentScore, int highScore)
    {
        gameObject.SetActive(true);

        _highScoreText.gameObject.SetActive(true);
        _currentScoreText.gameObject.SetActive(true);

        if (highScore < currentScore)
        {
            _highScoreText.SetText("New High Score : {0}", currentScore);
            _currentScoreText.gameObject.SetActive(false);
        }
        else
        {
            _highScoreText.SetText("High Score : {0}", highScore);
            _currentScoreText.SetText("Score : {0}", currentScore);
        }
    }
}