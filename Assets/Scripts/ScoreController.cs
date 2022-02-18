using System;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(EXEC_ORDER)]
public class ScoreController : MonoBehaviour
{
    public const int EXEC_ORDER = -1;

    public static event Action onMultiplierChanged;
    public static event Action onScoreChanged;

    const float SCORE_PERIOD = 5f;
    const float AVOIDANCE_MULTIPLIER_PERIOD = 2f;
    const float SURVIVAL_MULTIPLIER_PERIOD = 5f;
    const int MULT_PER_DESCENDANT = 1;
    const int MULT_PER_TNT = 10;
    const int SHOOT_TNT_POINTS = 3;
    const int SHOOT_STICKY_POINTS = 1;

    public static int Score => _i._score;
    public static int Multiplier => 1 + _i._avoidanceMultiplier + _i._survivalMultiplier + _i._stickyMultiplier;

    public static void AddPointsForShootingSomething(bool isTNT)
    {
        Assert.IsNotNull(_i);

        _i._score += (isTNT ? SHOOT_TNT_POINTS : SHOOT_STICKY_POINTS) * Multiplier;

        onScoreChanged?.Invoke();
    }

    static ScoreController _i;

    int _score;

    int _avoidanceMultiplier;
    int _survivalMultiplier;
    int _stickyMultiplier;
    float _nextScorePeriodAfter;
    float _nextAvoidanceMultiplierAfter;
    float _nextSurvivalMultiplierAfter;

    void OnEnable()
    {
        _i = this;

        _avoidanceMultiplier = _survivalMultiplier = 0;
        _nextScorePeriodAfter = Time.time + SCORE_PERIOD;
        _nextAvoidanceMultiplierAfter = Time.time + AVOIDANCE_MULTIPLIER_PERIOD;
        _nextSurvivalMultiplierAfter = Time.time + SURVIVAL_MULTIPLIER_PERIOD;

        Player.I.Sticky.onChildrenChanged += HandlePlayerStickiesChanged;
        HandlePlayerStickiesChanged();
    }

    void Update()
    {
        if (Player.Destroyed)
            return;

        var multiplierChanged = CheckAvoidanceMultiplier();
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
        _stickyMultiplier = sticky.NumDescendents * MULT_PER_DESCENDANT + sticky.NumTNTs * MULT_PER_TNT;
        CheckAvoidanceMultiplier();

        onMultiplierChanged?.Invoke();
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
