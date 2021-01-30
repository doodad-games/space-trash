using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    const float SPEED = 0.06f;

    public GameObject Owner => _owner.gameObject;

    Turret _owner;

    void FixedUpdate() =>
        transform.position += transform.right * SPEED;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        var turret = collision.collider.GetComponent<Turret>();
        if (turret == _owner)
            return;

        var soundPrefab = Resources.Load<GameObject>("Effects/TurretBulletCollideSound");
        Instantiate(soundPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Init(Turret owner) =>
        _owner = owner;
}
