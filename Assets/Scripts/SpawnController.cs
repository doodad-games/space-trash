using UnityEngine;

public class SpawnController : MonoBehaviour
{
    const float SPAWN_RETRY_DELAY = 0.5f;

    float x => Time.time - _startTime;

    float _startTime;
    float[] _nextSpawnAt = new float[2]; // Size of `SpawnType` enum

    void OnEnable()
    {
        _startTime = Time.time;

        for (var i = 0; i != _nextSpawnAt.Length; ++i)
            _nextSpawnAt[i] = 0;

        DeathTrash.onSpawnFailed += HandleDeathTrashSpawnFailed;
        DeathTrash.onTurretSpawnPrevented += HandleTurretSpawnFailed;
    }

    void FixedUpdate()
    {
        MaybeSpawn(SpawnType.DeathTrash);
        MaybeSpawn(SpawnType.Turret);
    }

    void OnDisable()
    {
        DeathTrash.onSpawnFailed -= HandleDeathTrashSpawnFailed;
        DeathTrash.onTurretSpawnPrevented -= HandleTurretSpawnFailed;
    }

    void MaybeSpawn(SpawnType type)
    {
        var iType = (int)type;
        var nextSpawnAt = _nextSpawnAt[iType];

        if (Time.time < nextSpawnAt)
            return;
        
        float nextSpawnDelay;

        if (type == SpawnType.DeathTrash)
        {
            nextSpawnDelay = DeathTrashSpawnDelay(x);
            DeathTrash.Spawn();
        }
        else
        {
            nextSpawnDelay = TurretSpawnDelay(x);
            Turret.Spawn();
        }

        _nextSpawnAt[iType] = Time.time + nextSpawnDelay;
    }

    float DeathTrashSpawnDelay(float x) =>
        Mathf.Max(
            0.5f,
            -Mathf.Log((x / 3600) + 0.1f) * 2
        );
    
    void HandleDeathTrashSpawnFailed()
    {
        var iType = (int)SpawnType.DeathTrash;
        _nextSpawnAt[iType] = Time.time + SPAWN_RETRY_DELAY;
    }
    
    float TurretSpawnDelay(float x) =>
        Mathf.Max(
            0.3f,
            -Mathf.Log((x / 2000) + 0.1f) * 2
        );
    
    void HandleTurretSpawnFailed()
    {
        var iType = (int)SpawnType.Turret;
        _nextSpawnAt[iType] = Time.time + SPAWN_RETRY_DELAY;
    }
    
    enum SpawnType
    {
        DeathTrash = 0,
        Turret = 1
    }
}
