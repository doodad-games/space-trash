using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    const float SCORE_PERIOD = 5f;
    const float AVOIDANCE_MULTIPLIER_PERIOD = 2.5f;
    const float LAZY_MULTIPLIER_PERIOD = 0.3f;
    const float SURVIVAL_MULTIPLIER_PERIOD = 7.5f;

    public static int Score => _i._score;

    static ScoreController _i;

#pragma warning disable CS0649
    [SerializeField] TextMeshPro _multiplierText;
    [SerializeField] TextMeshPro _scoreText;
#pragma warning restore CS0649

    int _score;

    int _avoidanceMultiplier;
    int _lazyMultiplier;
    int _survivalMultiplier;
    int _stickyMultiplier;
    float _nextScorePeriodAfter;
    float _nextAvoidanceMultiplierAfter;
    float _nextLazyMultiplierAfter;
    float _nextSurvivalMultiplierAfter;

    int Multiplier => 1 + _avoidanceMultiplier + _lazyMultiplier + _survivalMultiplier + _stickyMultiplier;

    void OnEnable()
    {
        _i = this;

        _avoidanceMultiplier = _lazyMultiplier = _survivalMultiplier = 0;
        _nextScorePeriodAfter = Time.time + SCORE_PERIOD;
        _nextAvoidanceMultiplierAfter = Time.time + AVOIDANCE_MULTIPLIER_PERIOD;
        _nextLazyMultiplierAfter = Time.time + LAZY_MULTIPLIER_PERIOD;
        _nextSurvivalMultiplierAfter = Time.time + SURVIVAL_MULTIPLIER_PERIOD;

        Player.I.Sticky.onChildrenChanged += HandlePlayerStickiesChanged;
        HandlePlayerStickiesChanged();

        Refresh();
    }

    void Update()
    {
        if (Player.Destroyed)
            return;

        var shouldRefresh = CheckAvoidanceMultiplier();
        shouldRefresh |= CheckLazyMultiplier();
        shouldRefresh |= CheckSurvivalMultiplier();
        shouldRefresh |= CheckScorePeriod();

        if (shouldRefresh)
            Refresh();
    }

    void OnDisable()
    {
        _i = null;

        if (
            Player.I != null &&
            Player.I.Sticky != null
        ) Player.I.Sticky.onChildrenChanged -= HandlePlayerStickiesChanged;
    }

    void HandlePlayerStickiesChanged()
    {
        var sticky = Player.I.Sticky;
        _stickyMultiplier = sticky.NumDescendents * 2 + sticky.NumTNTs * 5;
        Refresh();
    }

    bool CheckAvoidanceMultiplier()
    {
        if (Player.I.Sticky.HasChildren)
        {
            _nextAvoidanceMultiplierAfter = Time.time + AVOIDANCE_MULTIPLIER_PERIOD;

            if (_avoidanceMultiplier != 0)
            {
                _avoidanceMultiplier = 0;
                return true;
            }

            return false;
        }

        if (Time.time < _nextAvoidanceMultiplierAfter)
            return false;

        ++_avoidanceMultiplier;
        _nextAvoidanceMultiplierAfter += AVOIDANCE_MULTIPLIER_PERIOD;
        CheckAvoidanceMultiplier();

        return true;
    }

    bool CheckLazyMultiplier()
    {
        if (Player.JustReceivedInput)
        {
            _nextLazyMultiplierAfter = Time.time + LAZY_MULTIPLIER_PERIOD;

            if (_lazyMultiplier != 0)
            {
                _lazyMultiplier = 0;
                return true;
            }

            return false;
        }

        if (Time.time < _nextLazyMultiplierAfter)
            return false;

        ++_lazyMultiplier;
        _nextLazyMultiplierAfter += LAZY_MULTIPLIER_PERIOD;
        CheckLazyMultiplier();

        return true;
    }

    bool CheckSurvivalMultiplier()
    {
        if (Time.time < _nextSurvivalMultiplierAfter)
            return false;

        ++_survivalMultiplier;
        _nextSurvivalMultiplierAfter += SURVIVAL_MULTIPLIER_PERIOD;
        CheckSurvivalMultiplier();

        return true;
    }

    bool CheckScorePeriod()
    {
        if (Time.time < _nextScorePeriodAfter)
            return false;
        
        _score += Multiplier;
        
        _nextScorePeriodAfter += SCORE_PERIOD;
        CheckScorePeriod();

        return true;
    }

    void Refresh()
    {
        _multiplierText.text = Multiplier.ToString();
        _scoreText.text = _score.ToString();
    }
}
