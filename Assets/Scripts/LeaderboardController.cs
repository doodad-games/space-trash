using GooglePlayGames;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    public static LeaderboardController I { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        var obj = new GameObject("LeaderboardController");
        DontDestroyOnLoad(obj);
        obj.AddComponent<LeaderboardController>();
    }

    void OnEnable()
    {
        I = this;

#if GOOGLE_PLAY
        PlayGamesPlatform.Activate();
#endif

        MaybeAutoconnect();
        GameplayUI.onHighscoreChanged += MaybeUploadHighscore;
    }

    void OnDisable()
    {
        I = null;
        GameplayUI.onHighscoreChanged -= MaybeUploadHighscore;
    }

    void MaybeUploadHighscore()
    {
        var highscore = GameConfig.HighScore;
        if (highscore <= 0)
            return;
        
        UploadHighscore(highscore);
    }

#if !UNITY_IOS && !GOOGLE_PLAY
    public void ShowLeaderboard() {}

    void MaybeAutoconnect() {}
    void UploadHighscore(int highscore) {}
#endif

#if UNITY_IOS || GOOGLE_PLAY
    const string PP_SHOULD_AUTOCONNECT = "shouldAutoconnectLeaderboard";

#if UNITY_IOS
    const string LEADERBOARD_ID = "MainLeaderboard";
#else
    const string LEADERBOARD_ID = GPGSIds.leaderboard_trash_scores;
#endif

    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
            Social.ShowLeaderboardUI();
        else Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    MaybeUploadHighscore();
                    Social.ShowLeaderboardUI();
                }
            });
    }

    void MaybeAutoconnect()
    {
        var shouldAutoconnect = PlayerPrefs.GetInt(PP_SHOULD_AUTOCONNECT, 1) == 1;

        if (!shouldAutoconnect || Social.localUser.authenticated)
            return;

        Social.localUser.Authenticate(success =>
        {
            PlayerPrefs.SetInt(PP_SHOULD_AUTOCONNECT, success ? 1 : 0);
            if (success)
                MaybeUploadHighscore();
        });
    }

    void UploadHighscore(int highscore) =>
        Social.ReportScore(highscore, LEADERBOARD_ID, null);
#endif
}
