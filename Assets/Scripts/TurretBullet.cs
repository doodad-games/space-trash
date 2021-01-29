using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    const float SPEED = 0.06f;

    void FixedUpdate() =>
        transform.position += transform.right * SPEED;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        var soundPrefab = Resources.Load<GameObject>("Content/TurretBulletCollideSound");
        Instantiate(soundPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
