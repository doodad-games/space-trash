using UnityEngine;

public class DestroyWhenOffscreen : MonoBehaviour
{
    const float DISTANCE_FROM_PLAYER = -14f;

    void FixedUpdate() =>
        CheckForDestruction();
    
    void CheckForDestruction()
    {
        var myX = transform.position.x;
        var playerX = Player.I.transform.position.x;

        if (myX + DISTANCE_FROM_PLAYER < playerX)
            Destroy(gameObject);
    }
}
