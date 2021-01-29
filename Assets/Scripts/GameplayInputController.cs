using UnityEngine;

public class GameplayInputController
{
    public static int VerticalInput
    {
        get
        {
            if (
                Input.GetKey(KeyCode.UpArrow) ||
                Input.GetKey(KeyCode.W)
            ) return 1;

            if (
                Input.GetKey(KeyCode.DownArrow) ||
                Input.GetKey(KeyCode.S)
            ) return -1;

            return 0;
        }
    }

    public static int RotationalInput
    {
        get
        {
            if (
                Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.D)
            ) return 1;

            if (
                Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A)
            ) return -1;

            return 0;
        }
    }
}
