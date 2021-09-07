using UnityEngine;

public class SetAnchorForPlayerTorque : MonoBehaviour
{
    const float TORQUE_FACTOR = 4f;

    RectTransform _rect;

    void Awake() => _rect = (RectTransform)transform;
    void Update() =>
        Refresh();

    void Refresh()
    {
        var xPos = 
            Mathf.Clamp(
                (-Player.I.Torque + TORQUE_FACTOR) /
                    (TORQUE_FACTOR * 2),
                -0.25f,
                1.25f
            );
        var anchor = new Vector2(xPos, 0.5f);
        _rect.anchorMin = _rect.anchorMax = anchor;
    }
}
