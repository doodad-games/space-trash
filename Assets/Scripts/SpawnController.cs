using UnityEngine;

public class SpawnController : MonoBehaviour
{
    const float SPAWN_RETRY_DELAY = 0.5f;

    float x => Time.time - _startTime;

    float _startTime;
    float _nextDeathTrashSpawnAt;
    float _nextTurretSpawnAt;

    void OnEnable()
    {
        _startTime = Time.time;
        _nextDeathTrashSpawnAt = 0f;

        DeathTrash.onSpawnFailed += HandleDeathTrashSpawnFailed;
        DeathTrash.onTurretSpawnPrevented += HandleTurretSpawnFailed;
    }

    void FixedUpdate()
    {
        MaybeSpawnDeathTrash();
        MaybeSpawnTurret();
    }

    void OnDisable()
    {
        DeathTrash.onSpawnFailed -= HandleDeathTrashSpawnFailed;
        DeathTrash.onTurretSpawnPrevented -= HandleTurretSpawnFailed;
    }

    void MaybeSpawnDeathTrash()
    {
        if (Time.time < _nextDeathTrashSpawnAt)
            return;
        
        var nextSpawnDelay = DeathTrashSpawnDelay(x);
        _nextDeathTrashSpawnAt = Time.time + nextSpawnDelay;

        DeathTrash.Spawn();
    }

    float DeathTrashSpawnDelay(float x) =>
        Mathf.Max(
            0.5f,
            -Mathf.Log((x / 3600) + 0.1f) * 2
        );
    
    void HandleDeathTrashSpawnFailed() =>
        _nextDeathTrashSpawnAt = Time.time + SPAWN_RETRY_DELAY;
    
    void MaybeSpawnTurret()
    {
        if (Time.time < _nextTurretSpawnAt)
            return;
        
        var nextSpawnDelay = TurretSpawnDelay(x);
        _nextTurretSpawnAt = Time.time + nextSpawnDelay;

        Turret.Spawn();
    }

    float TurretSpawnDelay(float x) =>
        Mathf.Max(
            0.3f,
            -Mathf.Log((x / 2000) + 0.1f) * 2
        );
    
    void HandleTurretSpawnFailed() =>
        _nextTurretSpawnAt = Time.time + SPAWN_RETRY_DELAY;
}
