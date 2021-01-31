using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DefaultExecutionOrder(-1)]
public class Player : MonoBehaviour
{
    const float HORIZONTAL_VELOCITY = 1f;
    const float MINMAX_VERTICAL_VELOCITY = 4f;
    const float VERTICAL_VELOCITY_INC_STEP = 0.1f;
    const float VERTICAL_VELOCITY_OTHER_DIR_MULTIPLIER = 2.5f;
    const float MINMAX_Y_POS = 4f;
    const float TORQUE_INC_STEP = -0.05f;
    const float TORQUE_OTHER_DIR_MULTIPLIER = 2.5f;

    public static event Action onDestroyed;

    public static Player I { get; private set; }
    public static bool Destroyed => I._destroyed;
    public static bool JustReceivedInput => 
        I._prevVertInput != 0 || I._prevRotInput != 0;

    public Sticky Sticky => _sticky;

    Rigidbody2D _rb;
    Sticky _sticky;

    bool _destroyed;
    int _prevVertInput;
    int _prevRotInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sticky = GetComponent<Sticky>();
    }

    void OnEnable()
    {
        I = this;
        _sticky.torque = 2f;
        _prevVertInput = _prevRotInput = 0;

        _rb.velocity = new Vector2(HORIZONTAL_VELOCITY, 0);

        _sticky.onDestroyed += HandleStickyDestroyed;
    }

    void FixedUpdate()
    {
        ApplyUpDownInput();
        ClampUpDownPosition();
        ApplyRotationalInput();
    }

    void OnDisable() 
    {
        I = null;

        _sticky.onDestroyed -= HandleStickyDestroyed;
    }

    void ApplyUpDownInput()
    {
        var vertInput = GameplayInput.VerticalInput;
        if (vertInput != 0)
        {
            if (vertInput != _prevVertInput)
                SoundController.Play("thrusters");

            var step = VERTICAL_VELOCITY_INC_STEP * vertInput;
            if (Mathf.Sign(step) != Mathf.Sign(_rb.velocity.y))
                step *= VERTICAL_VELOCITY_OTHER_DIR_MULTIPLIER;

            var newVert = Mathf.Clamp(
                _rb.velocity.y + step,
                MINMAX_VERTICAL_VELOCITY * -1,
                MINMAX_VERTICAL_VELOCITY
            );

            _rb.velocity = new Vector2(_rb.velocity.x, newVert);
        }

        _prevVertInput = vertInput;
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
            if (rotInput != _prevRotInput)
                SoundController.Play("thrusters");

            var step = TORQUE_INC_STEP * rotInput;
            if (Mathf.Sign(step) != Mathf.Sign(_sticky.torque))
                step *= TORQUE_OTHER_DIR_MULTIPLIER;

            _sticky.torque += step;
        }

        _prevRotInput = rotInput;
    }

    void HandleStickyDestroyed()
    {
        SoundController.Play("player-boom");

        new Async(this)
            .Lerp(
                1f, 0f, 1f,
                (step) => Time.timeScale = step,
                TimeMode.Unscaled
            );

        _destroyed = true;
        onDestroyed?.Invoke();
    }
}
