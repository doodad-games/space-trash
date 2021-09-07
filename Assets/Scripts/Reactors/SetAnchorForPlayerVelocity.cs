using UnityEngine;

public class SetAnchorForPlayerVelocity : MonoBehaviour
{
    RectTransform _rect;

    void Awake() => _rect = (RectTransform)transform;
    void Update() =>
        Refresh();

    void Refresh()
    {
        var yPos = 
            (Player.I.Velocity.y + Player.MINMAX_VERTICAL_VELOCITY) /
                (Player.MINMAX_VERTICAL_VELOCITY * 2);
        var anchor = new Vector2(0.5f, yPos);
        _rect.anchorMin = _rect.anchorMax = anchor;
    }
}
