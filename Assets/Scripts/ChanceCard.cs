using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monopoly.Protobuf;

using AnimationPb = Monopoly.Protobuf.Animation;

public class ChanceCard : ForegroundDismissible
{

    [SerializeField] private Text Title;
    [SerializeField] private Text Description;
    [SerializeField] private AnimationDispatcher Anim;
    private Animator Animator;
    private Prisel.Protobuf.Packet PromptConfirmationRequest;
    private ChanceDisplay ChanceDisplay;
    private AnimationPb OpenChanceChestAnimation;

    protected override bool Initialize()
    {
        Anim.OnOpenChanceChest += OnOpenChanceChest;
        EventBus.PromptChanceConfirmation += OnPromptChanceConfirmation;
        Anim.OnDismissChanceCard += OnServerDismissChanceCard;
        Animator = GetComponent<Animator>();
        return true;
    }
    protected override void OnPrepare()
    {
        if (ChanceDisplay != null)
        {
            Title.text = ChanceDisplay.Title;
            Description.text = ChanceDisplay.Description;
            Animator.PlayForLength("ChanceCard_fly_out", OpenChanceChestAnimation.Length);
        }
    }

    protected override void Cleanup()
    {
        Anim.OnOpenChanceChest -= OnOpenChanceChest;
        EventBus.PromptChanceConfirmation -= OnPromptChanceConfirmation;
    }

    protected override void OnDismiss()
    {
        if (PromptConfirmationRequest != null)
        {
            Prisel.Protobuf.Packet request = PromptConfirmationRequest;
            PromptConfirmationRequest = null;
            EventBus.ConfirmChance?.Invoke(request);

        }
    }

    void OnPromptChanceConfirmation(Prisel.Protobuf.Packet packet)
    {
        SetTouchDismissible(true);
        PromptConfirmationRequest = packet;
    }

    void OnOpenChanceChest(AnimationPb animation)
    {
        Debug.Log("chance card received chance chest open animation!");
        if (animation.Extra.TryUnpack(out OpenChanceChestExtra extra))
        {
            ChanceDisplay = extra.Chance;
            OpenChanceChestAnimation = animation;
            Show();
        }
    }

    void OnServerDismissChanceCard(AnimationPb animation)
    {
        // Server dismissed the chance card, not need to respond to the
        // PromptConfirmationRequest anymore.
        PromptConfirmationRequest = null;
        EventBus.AutomaticallyDismissForeground?.Invoke();
    }


}
