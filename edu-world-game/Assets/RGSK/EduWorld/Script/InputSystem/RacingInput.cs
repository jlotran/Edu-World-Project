using Fusion;
using UnityEngine;

[System.Serializable]
public struct RacingInput : INetworkInput
{
    public int tick;
    public float throttleInput;
    public float steerInput;
    public float brakeInput;
    public float handbrakeInput;
    public float nitrousInput;
}
