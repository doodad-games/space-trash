using UnityEngine;

public class SpawnController : MonoBehaviour
{
    const float DEATH_TRASH_SPAWN_RETRY_DELAY = 0.5f;

    float _startTime;
    float _nextDeathTrashSpawnAt;

    void OnEnable()
    {
        _startTime = Time.time;
        _nextDeathTrashSpawnAt = 0f;

        DeathTrash.onSpawnFailed += HandleDeathTrashSpawnFailed;
    }

    void FixedUpdate()
    {
        MaybeSpawnDeathTrash();
    }

    void OnDisable() =>
        DeathTrash.onSpawnFailed -= HandleDeathTrashSpawnFailed;

    void MaybeSpawnDeathTrash()
    {
        if (Time.time < _nextDeathTrashSpawnAt)
            return;
        
        var x = Time.time - _startTime;
        var nextSpawnDelay = DeathTrashSpawnDelay(x);
        _nextDeathTrashSpawnAt = Time.time + nextSpawnDelay;

        DeathTrash.Spawn();
    }

    float DeathTrashSpawnDelay(float x) =>
        Mathf.Max(
            0.5f,
            -Mathf.Log((x / 3600) + 0.1f)
        );
    
    void HandleDeathTrashSpawnFailed() =>
        _nextDeathTrashSpawnAt = Time.time + DEATH_TRASH_SPAWN_RETRY_DELAY;
}
