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

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    
    void HandlePlayerDestroyed() =>
        _anim.SetTrigger("GameOver");
}
