using UnityEngine;

public class GameplayInput
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

            if (MobileInputButton.active)
            {
                var y = 0;
                foreach (var input in MobileInputButton.ActiveInputs)
                    y += input.Direction.y;
                return y;
            }

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

            if (MobileInputButton.active)
            {
                var x = 0;
                foreach (var input in MobileInputButton.ActiveInputs)
                    x += input.Direction.x;
                return x;
            }

            return 0;
        }
    }
}
