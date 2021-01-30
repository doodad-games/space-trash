using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    Animator _anim;

    void Awake() => _anim = GetComponent<Animator>();

    public void HandlePlayPressed() =>
        _anim.SetTrigger("Play");

    public void HandleExitPressed() 
    {
        SoundController.Play("game-over");
        _anim.SetTrigger("Exit");
    }

    public void ActuallyPlay() =>
        SceneManager.LoadScene("Gameplay");

    public void ActuallyExit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif

        Application.Quit();
    }
}