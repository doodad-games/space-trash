using System;
using UnityEngine;

public class DeathTrash : MonoBehaviour
{
    public static event Action onSpawnFailed;
    public static event Action onTurretSpawnPrevented;

    int _id;

    int _spawnCheckLayer;

    void OnEnable() =>
        _id = GameConfig.NewID;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == GameConfig.SpawnCheckLayer)
        {
            var otherDeathTrash = other.GetComponentInParent<DeathTrash>();
            if (otherDeathTrash != null)
            {
                if (_id > otherDeathTrash._id)
                    return;

                Destroy(otherDeathTrash.gameObject);

                onSpawnFailed?.Invoke();
            }
            else
            {
                var otherTurret = other.GetComponentInParent<Turret>();
                if (
                    otherTurret == null ||
                    otherTurret.Stable
                ) return;

                Destroy(otherTurret.gameObject);
                onTurretSpawnPrevented?.Invoke();
            }
        }
    }
}
