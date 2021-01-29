using System;
using UnityEngine;

public class DeathTrash : MonoBehaviour
{
    const float MINMAX_Y_RANGE = 4f;
    const float X_SPAWN_OFFSET = 18f;

    public static event Action onSpawnFailed;

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

    int _createdAt;
    int _spawnCheckLayer;

    void OnEnable() =>
        _createdAt = Time.frameCount;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == GameConfig.SpawnCheckLayer)
        {
            var otherDeathTrash = other.GetComponentInParent<DeathTrash>();
            if (otherDeathTrash._createdAt < _createdAt)
                return;

            Destroy(otherDeathTrash.gameObject);
            onSpawnFailed?.Invoke();
        }
    }
}
