using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    const string PP_HIGHSCORE = "highScore";

#pragma warning disable CS0649
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _highScoreText;
#pragma warning restore CS0649

    Animator _anim;

    void Awake() =>
        _anim = GetComponent<Animator>();

    void OnEnable() =>
        Player.onDestroyed += HandlePlayerDestroyed;
    void OnDisable() =>
        Player.onDestroyed -= HandlePlayerDestroyed;

    public void FadeInMusic() =>
        GameConfig.FadeMusicVolume(true);
    
    public void FadeOutMusic() =>
        GameConfig.FadeMusicVolume(false);
    
    public void HandleBackToMenuPressed() =>
        _anim.SetTrigger("BackToMenu");

    public void HandleCreditsPressed() =>
        _anim.SetBool("Credits", true);

    public void HandleBackToGameOverPressed() =>
        _anim.SetBool("Credits", false);

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    
    void HandlePlayerDestroyed()
    {
        _anim.SetBool("IsEndGame", true);

        var highScore = PlayerPrefs.GetInt(PP_HIGHSCORE, -1);
        if (ScoreController.Score > highScore)
        {
            highScore = ScoreController.Score;
            PlayerPrefs.SetInt(PP_HIGHSCORE, highScore);
        }

        _scoreText.text = ScoreController.Score.ToString();
        _highScoreText.text = highScore.ToString();
    }
}
