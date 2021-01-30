using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public bool Stable => _stable;

#pragma warning disable CS0649
    [SerializeField] Transform _bulletSpawnPoint;
#pragma warning restore CS0649

    bool _stable;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        _stable = true;
    }

    public void SpawnBullet()
    {
        var spawnPos = _bulletSpawnPoint.position;

        var prefab = Resources.Load<GameObject>("Content/TurretBullet");
        var bulletObj = Instantiate(prefab, spawnPos, _bulletSpawnPoint.rotation);
        bulletObj.GetComponent<TurretBullet>()
            .Init(this);

        var spawnSoundPrefab = Resources.Load<GameObject>("Effects/TurretBulletSpawnSound");
        Instantiate(spawnSoundPrefab, spawnPos, Quaternion.identity);
    }
}
