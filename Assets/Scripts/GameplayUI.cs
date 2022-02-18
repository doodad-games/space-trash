using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    public static event Action onHighscoreChanged;

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
    
    public void HandleAgainButtonPressed() =>
        _anim.SetTrigger("Again");

    public void HandleCreditsPressed() =>
        _anim.SetBool("Credits", true);

    public void HandleBackToGameOverPressed() =>
        _anim.SetBool("Credits", false);

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Again()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
    
    void HandlePlayerDestroyed()
    {
        _anim.SetBool("IsEndGame", true);

        var highScore = GameConfig.HighScore;
        if (ScoreController.Score > highScore)
        {
            GameConfig.HighScore = ScoreController.Score;
            onHighscoreChanged?.Invoke();
        }

        _scoreText.text = ScoreController.Score.ToString();
        _highScoreText.text = highScore.ToString();
    }
}
