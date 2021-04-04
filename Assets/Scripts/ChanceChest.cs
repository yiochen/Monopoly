using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monopoly.Protobuf;

using AnimationPb = Monopoly.Protobuf.Animation;

public class ChanceChest : MonoBehaviour
{
    [SerializeField] private EventBus EventBus;
    [SerializeField] private AnimationDispatcher Anim;

    public Coordinate Coordinate { get; set; }

    private Animator Animator;

    void OnEnable()
    {
        Animator = GetComponent<Animator>();
        Anim.OnOpenChanceChest += OnOpenChanceChest;
    }

    void OnDisable()
    {
        Anim.OnOpenChanceChest -= OnOpenChanceChest;
    }

    void OnOpenChanceChest(AnimationPb anim)
    {
        Debug.Log("Playing chance chest");
        if (anim.Extra.TryUnpack(out Monopoly.Protobuf.OpenChanceChestExtra extra))
        {
            if (extra.ChanceChestTile.Equals(Coordinate))
            {
                Animator.PlayForLength("ChanceChest_open", anim.Length);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
