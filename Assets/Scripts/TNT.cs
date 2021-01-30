using UnityEngine;

public class TNT : MonoBehaviour
{
    const float MINMAX_Y_RANGE = 4f;
    const float X_SPAWN_OFFSET = 25f;
    const float MAX_SPAWN_X_VELOCITY = 1f;
    const float MINMAX_SPAWN_Y_VELOCITY = 0.1f;
    const float MINMAX_SPAWN_TORQUE = 1f;

    public static void Spawn()
    {
        var prefabToSpawn = Resources.Load<GameObject>("Content/TNT");

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

    Rigidbody2D _rb;
    Sticky _sticky;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sticky = GetComponent<Sticky>();
    }

    void OnEnable()
    {
        _rb.velocity = new Vector2(
            Random.Range(-MAX_SPAWN_X_VELOCITY, 0f),
            Random.Range(-MINMAX_SPAWN_Y_VELOCITY, MINMAX_SPAWN_Y_VELOCITY)
        );
        _sticky.torque = Random.Range(-MINMAX_SPAWN_TORQUE, MINMAX_SPAWN_TORQUE);
    }
}
