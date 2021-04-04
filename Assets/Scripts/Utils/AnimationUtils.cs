using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationUtils
{
    private static readonly string SPEED_PARAMETER = "SpeedMultiplier";
    private static readonly float DEFAULT_ANIMATION_LENGTH = 1000f;
    public static IEnumerator WaitStart(this Animator animator, int layer, string state, System.Action callback)
    {
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(state))
        {
            yield return null;
        }
        callback.Invoke();
    }

    /// <summary>
    /// Play the animation for the given length by setting the SpeedMultiplier parameter.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="state"></param>
    /// <param name="serverLengthInMs"></param>
    /// <param name="layer"></param>
    public static void PlayForLength(this Animator animator, string state, int serverLengthInMs, int layer = 0)
    {
        animator.SetFloat(SPEED_PARAMETER, DEFAULT_ANIMATION_LENGTH / serverLengthInMs);
        animator.Play(state, layer);
    }

    public static void ResetSpeed(this Animator animator)
    {
        animator.SetFloat(SPEED_PARAMETER, 1f);
    }
}
