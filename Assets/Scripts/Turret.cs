using UnityEngine;

public class Turret : MonoBehaviour
{
    const float MINMAX_Y_RANGE = 4f;
    const float X_SPAWN_OFFSET = 25f;

    public static void Spawn()
    {
        var prefabToSpawn = Resources.Load<GameObject>("Content/Turret");

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

    public int CreatedAt => _createdAt;

#pragma warning disable CS0649
    [SerializeField] Transform _bulletSpawnPoint;
#pragma warning restore CS0649

    int _createdAt;

    void OnEnable() =>
        _createdAt = Time.frameCount;

    public void SpawnBullet()
    {
        var spawnPos = _bulletSpawnPoint.position;

        var prefab = Resources.Load<GameObject>("Content/TurretBullet");
        Instantiate(prefab, spawnPos, _bulletSpawnPoint.rotation);

        var spawnSoundPrefab = Resources.Load<GameObject>("Content/TurretBulletSpawnSound");
        Instantiate(spawnSoundPrefab, spawnPos, Quaternion.identity);
    }
}
