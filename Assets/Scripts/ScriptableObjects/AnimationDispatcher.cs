using System;
using Monopoly.Protobuf;

[UnityEngine.CreateAssetMenu(menuName = V)]
public class AnimationDispatcher : UnityEngine.ScriptableObject
{
    private const string V = "Settings/AnimationDispatcher";

    public Action<Animation> OnTurnStart;
    public Action<Animation> OnDiceDown;
    public Action<Animation> OnMove;
    public Action<Animation> OnTeleportPickup;
    public Action<Animation> OnTeleportDropoff;

}
