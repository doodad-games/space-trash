using UnityEngine;

public class SpawnController : MonoBehaviour
{
    const float SPAWN_RETRY_DELAY = 0.5f;

    float x => Time.time - _startTime;

    float _startTime;
    float[] _nextSpawnAt = new float[3]; // Size of `SpawnType` enum

    void OnEnable()
    {
        _startTime = Time.time;

        for (var i = 0; i != _nextSpawnAt.Length; ++i)
            _nextSpawnAt[i] = 0;
        
        var tntI = (int)SpawnType.TNT;
        _nextSpawnAt[tntI] = TNTSpawnDelay(x);

        DeathTrash.onSpawnFailed += HandleDeathTrashSpawnFailed;
        DeathTrash.onTurretSpawnPrevented += HandleTurretSpawnFailed;
    }

    void FixedUpdate()
    {
        MaybeSpawn(SpawnType.DeathTrash);
        MaybeSpawn(SpawnType.Turret);
        MaybeSpawn(SpawnType.TNT);
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
        else if (type == SpawnType.Turret)
        {
            nextSpawnDelay = TurretSpawnDelay(x);
            Turret.Spawn();
        }
        else
        {
            nextSpawnDelay = TNTSpawnDelay(x);
            TNT.Spawn();
        }

        _nextSpawnAt[iType] = Time.time + nextSpawnDelay;
    }

    float DeathTrashSpawnDelay(float x) =>
        Mathf.Max(
            0.5f,
            -Mathf.Log((x / 3600) + 0.1f) * 2
        );
    
    float TurretSpawnDelay(float x) =>
        Mathf.Max(
            0.3f,
            -Mathf.Log((x / 2000) + 0.1f) * 2
        );

    float TNTSpawnDelay(float x) =>
        Mathf.Max(
            0.5f,
            -Mathf.Log((x / 2000) + 0.1f) * 8
        );
    
    void HandleDeathTrashSpawnFailed()
    {
        var iType = (int)SpawnType.DeathTrash;
        _nextSpawnAt[iType] = Time.time + SPAWN_RETRY_DELAY;
    }
    
    void HandleTurretSpawnFailed()
    {
        var iType = (int)SpawnType.Turret;
        _nextSpawnAt[iType] = Time.time + SPAWN_RETRY_DELAY;
    }
    
    enum SpawnType
    {
        DeathTrash = 0,
        Turret = 1,
        TNT = 2
    }
}
