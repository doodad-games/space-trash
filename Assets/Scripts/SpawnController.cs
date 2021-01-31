using UnityEngine;

public class SpawnController : MonoBehaviour
{
    const float SPAWN_RETRY_DELAY = 0.5f;
    const float MINMAX_Y_RANGE = 4f;
    const float X_SPAWN_OFFSET = 25f;
    const float MAX_SPAWN_X_VELOCITY = 0.75f;
    const float MINMAX_SPAWN_Y_VELOCITY = 0.2f;
    const float MINMAX_SPAWN_TORQUE = 0.75f;

    float x => Time.time - _startTime;

    float _startTime;
    float[] _nextSpawnAt;

    void OnEnable()
    {
        _startTime = Time.time;

        _nextSpawnAt = new float[6]; // Size of `SpawnType` enum
        for (var i = 0; i != _nextSpawnAt.Length; ++i)
            _nextSpawnAt[i] = 0;
        
        var tntI = (int)SpawnType.TNT;
        _nextSpawnAt[tntI] = TNTSpawnDelay(x);

        DeathTrash.onSpawnFailed += HandleDeathTrashSpawnFailed;
        DeathTrash.onTurretSpawnPrevented += HandleTurretSpawnFailed;
    }

    void FixedUpdate()
    {
        MaybeSpawn(SpawnType.TrashDeath);
        MaybeSpawn(SpawnType.Turret);
        MaybeSpawn(SpawnType.TNT);
        MaybeSpawn(SpawnType.TrashSmall);
        MaybeSpawn(SpawnType.TrashMedium);
        MaybeSpawn(SpawnType.TrashLarge);
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

        if (type == SpawnType.TrashDeath)
        {
            nextSpawnDelay = Mathf.Max(
                0.5f,
                -Mathf.Log((x / 800) + 0.1f) * 2
            );

            var resourceLoc = GameConfig.ResourceBuckets["TrashDeath"].PickRandom();
            var prefabToSpawn = Resources.Load<GameObject>(resourceLoc);
            var xPos = Player.I.transform.position.x + X_SPAWN_OFFSET;
            var yPos = UnityEngine.Random.Range(-MINMAX_Y_RANGE, MINMAX_Y_RANGE) * 1.3f;
            var spawnPos = new Vector3(xPos, yPos, 0);

            var spawnRot = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));

            var obj = Instantiate(
                prefabToSpawn,
                spawnPos,
                spawnRot
            );
            obj.transform.localScale = Vector3.one * 1.2f;
        }
        else if (type == SpawnType.Turret)
        {
            nextSpawnDelay = Mathf.Max(
                0.3f,
                -Mathf.Log((x / 400) + 0.1f)
            );

            var prefabToSpawn = Resources.Load<GameObject>("Content/Turret");
            Spawn(prefabToSpawn);
        }
        else if (type == SpawnType.TNT)
        {
            nextSpawnDelay = TNTSpawnDelay(x);

            var prefabToSpawn = Resources.Load<GameObject>("Content/TNT");
            var obj = Spawn(prefabToSpawn);
            ApplyInitialMomentum(obj, 1.2f);
        } else if (type == SpawnType.TrashSmall)
        {
            nextSpawnDelay = 0.65f;

            var resourceLoc = GameConfig.ResourceBuckets["TrashSmall"].PickRandom();
            var prefabToSpawn = Resources.Load<GameObject>(resourceLoc);
            var obj = Spawn(prefabToSpawn);
            ApplyInitialMomentum(obj, 1.5f);
        }
        else if (type == SpawnType.TrashMedium)
        {
            nextSpawnDelay = 2.5f;

            var resourceLoc = GameConfig.ResourceBuckets["TrashMedium"].PickRandom();
            var prefabToSpawn = Resources.Load<GameObject>(resourceLoc);
            var obj = Spawn(prefabToSpawn);
            ApplyInitialMomentum(obj, 1.2f);
        }
        else
        {
            nextSpawnDelay = 4f;

            var resourceLoc = GameConfig.ResourceBuckets["TrashLarge"].PickRandom();
            var prefabToSpawn = Resources.Load<GameObject>(resourceLoc);
            var obj = Spawn(prefabToSpawn);
            ApplyInitialMomentum(obj, 0.8f);
        }

        _nextSpawnAt[iType] = Time.time + nextSpawnDelay;
    }

    float TNTSpawnDelay(float x) =>
        Mathf.Max(
            0.5f,
            -Mathf.Log((x / 400) + 0.1f)
        );

    GameObject Spawn(GameObject prefabToSpawn)
    {
        var xPos = Player.I.transform.position.x + X_SPAWN_OFFSET;
        var yPos = UnityEngine.Random.Range(-MINMAX_Y_RANGE, MINMAX_Y_RANGE);
        var spawnPos = new Vector3(xPos, yPos, 0);

        var spawnRot = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));

        return Instantiate(
            prefabToSpawn,
            spawnPos,
            spawnRot
        );
    }

    void ApplyInitialMomentum(GameObject spawnedObj, float magnitude)
    {
        var rb = spawnedObj.GetComponent<Rigidbody2D>();
        var sticky = spawnedObj.GetComponent<Sticky>();

        rb.velocity = new Vector2(
            Random.Range(-MAX_SPAWN_X_VELOCITY, 0f) * magnitude,
            Random.Range(-MINMAX_SPAWN_Y_VELOCITY, MINMAX_SPAWN_Y_VELOCITY)
        );
        sticky.torque = Random.Range(-MINMAX_SPAWN_TORQUE, MINMAX_SPAWN_TORQUE) * magnitude;
    }
    
    void HandleDeathTrashSpawnFailed()
    {
        var iType = (int)SpawnType.TrashDeath;
        _nextSpawnAt[iType] = Time.time + SPAWN_RETRY_DELAY;
    }
    
    void HandleTurretSpawnFailed()
    {
        var iType = (int)SpawnType.Turret;
        _nextSpawnAt[iType] = Time.time + SPAWN_RETRY_DELAY;
    }
    
    enum SpawnType
    {
        TrashDeath = 0,
        Turret = 1,
        TNT = 2,
        TrashSmall = 3,
        TrashMedium = 4,
        TrashLarge = 5
    }
}
