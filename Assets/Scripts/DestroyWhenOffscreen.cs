using UnityEngine;

public class DestroyWhenOffscreen : MonoBehaviour
{
    const float MIN_X_DISTANCE_FROM_PLAYER = -13f;
    const float MAX_X_DISTANCE_FROM_PLAYER = 25f;
    const float MINMAX_Y_DISTANCE = 16f;

    void FixedUpdate() =>
        CheckForDestruction();
    
    void CheckForDestruction()
    {
        var pos = transform.position;
        var myX = pos.x;
        var playerX = Player.I.transform.position.x;

        if (
            myX < playerX + MIN_X_DISTANCE_FROM_PLAYER ||
            myX > playerX + MAX_X_DISTANCE_FROM_PLAYER ||
            Mathf.Abs(pos.y) > MINMAX_Y_DISTANCE
        ) Destroy(gameObject);
    }
}
