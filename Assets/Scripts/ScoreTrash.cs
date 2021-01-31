using UnityEngine;

public class ScoreTrash : MonoBehaviour
{
    Vector3 _driftDirection;
    Quaternion _rotator;

    void OnEnable()
    {
        _driftDirection = Random.insideUnitCircle * 0.02f;

        var rotationDirection = Random.value > 0.5 ? 1 : -1;
        var rotationAmount = Random.Range(0.04f, 0.06f);
        _rotator = Quaternion.Euler(0, 0, rotationAmount * rotationDirection);
    }

    void Update()
    {
        transform.position += _driftDirection;
        transform.rotation *= _rotator;
    }
}