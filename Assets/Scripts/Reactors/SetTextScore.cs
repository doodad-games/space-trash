using TMPro;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class SetTextScore : MonoBehaviour
{
    public const int EXEC_ORDER = ScoreController.EXEC_ORDER + 1;

    TextMeshPro _worldText;
    TextMeshProUGUI _uiText;
    
    void Awake()
    {
        _worldText = GetComponent<TextMeshPro>();
        _uiText = GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        ScoreController.onScoreChanged += Refresh;
        Refresh();
    }
    void OnDisable() =>
        ScoreController.onScoreChanged -= Refresh;

    void Refresh()
    {
        var text = ScoreController.Score.ToString();
        if (_worldText != null)
            _worldText.text = text;
        if (_uiText != null)
            _uiText.text = text;
    }
}

