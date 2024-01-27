using Fusion;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public NetworkButtons Buttons;
}

