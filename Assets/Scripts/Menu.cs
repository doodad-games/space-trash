using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    static (string, string)[] _trashTalks = new (string, string)[]
    {
        (
            "This game is trash",
            "-New York Times"
        ),
        (
            "It's time to take out the trash",
            "-George the Garbage Man"
        ),
        (
            "Don't buy this. Your money will go to waste",
            "-The Washington Post"
        ),
        (
            "This isn't the time for trash talk",
            "-Mohamad Akil"
        ),
        (
            "This thing's just a hunk-o'junk!",
            "-Daily Telegraph"
        ),
        (
            "After playing Space Trash, I felt down in the dumps",
            "-Chicago Tribune"
        ),
        (
            "An exhilarating journey, litter-alley",
            "-A Pun-dit"
        ),
        (
            "I like to play this when I'm totally trashed",
            "-Charlie Sheen"
        ),
        (
            "I feel like I'm throwing my life away",
            "-Suicide Squad"
        ),
        (
            "I took it and I threw it on the ground",
            "-The Lonely Island"
        ),
        (
            "Break yourself upon my body",
            "-Ozruk"
        ),
        (
            "Don't get too attached",
            "-The Guardian"
        ),
        (
            "When I think of trash, I think of Doodad Games",
            "-Blizzard Entertainment"
        )
    };

#pragma warning disable CS0649
    [SerializeField] TextMeshProUGUI _trashTalkLine1;
    [SerializeField] TextMeshProUGUI _trashTalkLine2;
#pragma warning restore CS0649

    Animator _anim;
    int _trashTalkI;

    void Awake() => _anim = GetComponent<Animator>();

    void OnEnable() => _trashTalkI = 0;

    public void HandlePlayPressed() =>
        _anim.SetTrigger("Play");

    public void HandleExitPressed() 
    {
        SoundController.Play("player-boom");
        _anim.SetTrigger("Exit");
    }

    public void ActuallyPlay() =>
        SceneManager.LoadScene("Gameplay");

    public void FadeInMusic() =>
        GameConfig.FadeMusicVolume(true);

    public void FadeOutMusic() =>
        GameConfig.FadeMusicVolume(false);
    
    public void RefreshTrashTalk()
    {
        _trashTalkI = (_trashTalkI + 1) % _trashTalks.Length;
        var (line1, line2) = _trashTalks[_trashTalkI];
        _trashTalkLine1.text = '"' + line1 + '"';
        _trashTalkLine2.text = "    " + line2;
    }

    public void ActuallyExit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif

        Application.Quit();
    }
}
