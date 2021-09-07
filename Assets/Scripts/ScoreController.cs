using System;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class ScoreController : MonoBehaviour
{
    public const int EXEC_ORDER = -1;

    public static event Action onMultiplierChanged;
    public static event Action onScoreChanged;

    const float SCORE_PERIOD = 5f;
    const float AVOIDANCE_MULTIPLIER_PERIOD = 2.5f;
    const float LAZY_MULTIPLIER_PERIOD = 0.3f;
    const float SURVIVAL_MULTIPLIER_PERIOD = 7.5f;

    public static int Score => _i._score;
    public static int Multiplier => 1 + _i._avoidanceMultiplier + _i._lazyMultiplier + _i._survivalMultiplier + _i._stickyMultiplier;

    static ScoreController _i;

    int _score;

    int _avoidanceMultiplier;
    int _lazyMultiplier;
    int _survivalMultiplier;
    int _stickyMultiplier;
    float _nextScorePeriodAfter;
    float _nextAvoidanceMultiplierAfter;
    float _nextLazyMultiplierAfter;
    float _nextSurvivalMultiplierAfter;

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
    }

    void Update()
    {
        if (Player.Destroyed)
            return;

        var multiplierChanged = CheckAvoidanceMultiplier();
        multiplierChanged |= CheckLazyMultiplier();
        multiplierChanged |= CheckSurvivalMultiplier();

        if (multiplierChanged)
            onMultiplierChanged?.Invoke();

        if (CheckScorePeriod())
            onScoreChanged?.Invoke();
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
}
