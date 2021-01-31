using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
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
    
    void HandlePlayerDestroyed() =>
        _anim.SetBool("IsEndGame", true);
}
