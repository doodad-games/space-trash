using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    const float SPEED = 0.09f;

    public GameObject Owner => _owner == null ? null : _owner.gameObject;
    public bool WasShotFromPlayer => _wasShotFromPlayer;

    Turret _owner;
    bool _wasShotFromPlayer;

    void FixedUpdate() =>
        transform.position += transform.right * SPEED;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        var turret = collision.collider.GetComponent<Turret>();
        if (_owner != null && turret == _owner)
            return;

        var soundPrefab = Resources.Load<GameObject>("Effects/TurretBulletSoundCollide");
        Instantiate(soundPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);

        if (collision.collider.GetComponent<Sticky>() == null)
            Visuals.Spawn(transform.position, "EffectVisualsThud");
    }

    public void Init(Turret owner)
    {
        _owner = owner;
        _wasShotFromPlayer = _owner.GetComponent<Sticky>().IsConnectedToPlayer;
    }
}
