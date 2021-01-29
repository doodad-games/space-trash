using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    const float HORIZONTAL_VELOCITY = 1f;
    const float MINMAX_VERTICAL_VELOCITY = 3.5f;
    const float VERTICAL_VELOCITY_INC_STEP = 0.05f;
    const float MINMAX_Y_POS = 4f;
    const float TORQUE_INC_STEP = -0.05f;
    const float TORQUE_OTHER_DIR_MULTIPLIER = 2.5f;

    public static Player I { get; private set; }

    Rigidbody2D _rb;

    float _torque;

    void Awake() => _rb = GetComponent<Rigidbody2D>();

    void OnEnable()
    {
        I = this;
        _rb.velocity = new Vector2(HORIZONTAL_VELOCITY, 0);
        _torque = 0f;
    }

    void FixedUpdate()
    {
        ApplyUpDownInput();
        ClampUpDownPosition();
        ApplyRotationalInput();
    }

    void OnDisable() => I = null;

    void ApplyUpDownInput()
    {
        var vertInput = GameplayInput.VerticalInput;
        if (vertInput == 0)
            return;

        var curVert = _rb.velocity.y;
        var newVert = Mathf.Clamp(
            _rb.velocity.y + VERTICAL_VELOCITY_INC_STEP * vertInput,
            MINMAX_VERTICAL_VELOCITY * -1,
            MINMAX_VERTICAL_VELOCITY
        );

        _rb.velocity = new Vector2(_rb.velocity.x, newVert);
    }

    void ClampUpDownPosition()
    {
        var pos = transform.position;
        if (Mathf.Abs(pos.y) > MINMAX_Y_POS)
        {
            if (pos.y > MINMAX_Y_POS)
                pos.y = MINMAX_Y_POS;
            else
                pos.y = -MINMAX_Y_POS;

            transform.position = pos;
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
        }
    }

    void ApplyRotationalInput()
    {
        var rotInput = GameplayInput.RotationalInput;
        if (rotInput != 0)
        {
            var step = TORQUE_INC_STEP * rotInput;
            if (Mathf.Sign(step) != Mathf.Sign(_torque))
                step *= TORQUE_OTHER_DIR_MULTIPLIER;

            _torque += step;
        }

        _rb.rotation += _torque;
    }
}
