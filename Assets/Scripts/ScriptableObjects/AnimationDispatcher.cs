using System;
using Monopoly.Protobuf;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

using AnimationPb = Monopoly.Protobuf.Animation;

[UnityEngine.CreateAssetMenu(menuName = V)]
public class AnimationDispatcher : UnityEngine.ScriptableObject
{
    private const string V = "Settings/AnimationDispatcher";

    public Action<AnimationPb> OnTurnStart;
    public Action<AnimationPb> OnDiceDown;
    public Action<AnimationPb> OnMove;
    public Action<AnimationPb> OnTeleportPickup;
    public Action<AnimationPb> OnTeleportDropoff;
    public Action<AnimationPb> OnGameStart;
    public Action<AnimationPb> OnDiceRoll;
    public Action<AnimationPb> OnFocusLand;
    public Action<AnimationPb> OnInvested;
    public Action<AnimationPb> OnPan;
    public Action<AnimationPb> OnPayRent;
    public Action<AnimationPb> OnPlayerEmotion;

    /// <summary>
    /// the time it takes for the card to fly out of the chest to the center of screen
    /// </summary>
    public Action<AnimationPb> OnOpenChanceChest;
    public Action<AnimationPb> OnDismissChanceCard;



    private void DispatchDefaultAnimation(AnimationPb anim)
    {
        switch (anim.Name)
        {
            case "turn_start":
                OnTurnStart?.Invoke(anim);
                break;
            case "dice_roll":
                OnDiceRoll?.Invoke(anim);
                break;
            case "dice_down":
                OnDiceDown?.Invoke(anim);
                break;
            case "move":
                OnMove?.Invoke(anim);
                break;
            case "teleport_pickup":
                OnTeleportPickup?.Invoke(anim);
                break;
            case "teleport_dropoff":
                OnTeleportDropoff?.Invoke(anim);
                break;
            case "focus_land":
                OnFocusLand?.Invoke(anim);
                break;
            case "invested":
                OnInvested?.Invoke(anim);
                break;
            case "pan":
                OnPan?.Invoke(anim);
                break;
            case "pay_rent":
                OnPayRent?.Invoke(anim);
                break;
            case "player_emotion":
                OnPlayerEmotion?.Invoke(anim);
                break;
            case "open_chance_chest":
                OnOpenChanceChest?.Invoke(anim);
                break;
            case "dismiss_chance_card":
                OnDismissChanceCard?.Invoke(anim);
                break;
            case "game_start":
                OnGameStart?.Invoke(anim);
                break;
            default:
                Debug.LogWarning($"Received unrecognized animation {anim.Name}");
                break;
        }
    }
    public async Task HandleAnimation(AnimationPb anim)
    {
        List<Task> childTasks = new List<Task>();

        switch (anim.Type)
        {
            case AnimationType.Default:
                Debug.Log($"server animation {anim.Name} length: {(float)anim.Length / 1000}");
                DispatchDefaultAnimation(anim);
                ;
                await Task.Delay(anim.Length);
                break;
            case AnimationType.Sequence:
                foreach (var child in anim.Children)
                {
                    await HandleAnimation(child);
                }
                break;
            case AnimationType.Race:
                foreach (var child in anim.Children)
                {
                    childTasks.Add(HandleAnimation(child));
                }
                await Task.WhenAny(childTasks);
                break;
            case AnimationType.All:
                foreach (var child in anim.Children)
                {
                    childTasks.Add(HandleAnimation(child));
                }
                await Task.WhenAll(childTasks);
                break;
        }
    }

}
