using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    const float SPEED = 0.09f;

    public GameObject Owner
    {
        get
        {
            if (_owner == null)
                return null;
            return _owner.gameObject;
        }
    }

    Turret _owner;

    void FixedUpdate() =>
        transform.position += transform.right * SPEED;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        var turret = collision.collider.GetComponent<Turret>();
        if (turret == _owner)
            return;

        var soundPrefab = Resources.Load<GameObject>("Effects/TurretBulletSoundCollide");
        Instantiate(soundPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Init(Turret owner) =>
        _owner = owner;
}
