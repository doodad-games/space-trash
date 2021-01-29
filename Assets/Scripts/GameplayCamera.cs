using UnityEngine;

/* - Assumed to be a child of the player's vehicle, so inherits its position.
 * - Assumed that world Y position should always be 0, regardless of player's. 
 */

// Update the `CameraRectPositioner2D` before it updates itself
[DefaultExecutionOrder(-1)]

[RequireComponent(typeof(CameraRectPositioner2D))]
public class GameplayCamera : MonoBehaviour
{
    static Vector3 _playerOffset = new Vector3(5, 0, 0);
    static Vector2 _coreDisplaySize = new Vector2(16, 9);

    CameraRectPositioner2D _cameraPositioner;

    void Awake() =>
        _cameraPositioner = GetComponent<CameraRectPositioner2D>();

    void Update()
    {
        MoveToPlayer();
        UpdateCameraPositionerWorldRect();
    }

    void MoveToPlayer()
    {
        var pos = Player.I.transform.position + _playerOffset;

        if (!Mathf.Approximately(pos.y, 0))
            pos = new Vector3(pos.x, 0, 0);

        transform.position = pos;
    }

    
    void UpdateCameraPositionerWorldRect()
    {
        var bottomLeft = (Vector2)transform.position - _coreDisplaySize / 2f;
        _cameraPositioner.worldRect = new Rect(bottomLeft, _coreDisplaySize);
    }
}
