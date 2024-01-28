using Fusion;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Flags]
public enum InputButton
{
    LEFT = 1 << 0,
    RIGHT = 1 << 1,
    ATTACK = 1 << 3,
    JUMP = 1 << 4,
}

public struct InputData : INetworkInput
{
    public Vector3 direction;
    public NetworkButtons Buttons;
    public Vector3 mouseDir;

    public bool GetButton(InputButton button)
    {
        return Buttons.IsSet(button);
    }

    public NetworkButtons GetButtonPressed(NetworkButtons prev)
    {
        return Buttons.GetPressed(prev);
    }

    public bool AxisPressed()
    {
        return GetButton(InputButton.LEFT) || GetButton(InputButton.RIGHT);
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
        return mouseWorldPosition;
    }

}
