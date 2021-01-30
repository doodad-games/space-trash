using System;
using UnityEngine;

public class DeathTrash : MonoBehaviour
{
    const float MINMAX_Y_RANGE = 4f;
    const float X_SPAWN_OFFSET = 25f;

    public static event Action onSpawnFailed;
    public static event Action onTurretSpawnPrevented;

    public static void Spawn()
    {
        var resourceLoc = GameConfig.DeathTrashResourceNames.PickRandom();
        var prefabToSpawn = Resources.Load<GameObject>(resourceLoc);

        var xPos = Player.I.transform.position.x + X_SPAWN_OFFSET;
        var yPos = UnityEngine.Random.Range(-MINMAX_Y_RANGE, MINMAX_Y_RANGE);
        var spawnPos = new Vector3(xPos, yPos, 0);

        var spawnRot = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));

        Instantiate(
            prefabToSpawn,
            spawnPos,
            spawnRot
        );
    }

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
                if (otherDeathTrash._id > _id)
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
