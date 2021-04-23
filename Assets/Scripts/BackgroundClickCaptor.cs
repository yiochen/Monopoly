using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundClickCaptor : MonoBehaviour
{
    public enum BackgroundType
    {
        RewardBackground,
        NormalBackground
    }
    [SerializeField] private EventBus EventBus;
    [SerializeField] private AnimationDispatcher Anim;
    [SerializeField] private BackgroundType Background;

    enum State
    {
        INITIALIZING,
        DISMISSED,
        SHOWING,
    }

    private State BackgroundState = State.INITIALIZING;
    private bool TouchDismissible = true;

    void Awake()
    {
        if (Background == BackgroundType.RewardBackground)
        {
            EventBus.DisplayRewardBackground += OnDisplayBackground;
        }
        else
        {
            EventBus.DisplayNormalBackground += OnDisplayBackground;
        }
        EventBus.AutomaticallyDismissForeground += OnAutomaticallyDismissed;
        EventBus.SetBackgroundTouchEnabled += OnSetBackgroundTouchEnabled;
        gameObject.SetActive(false);
        BackgroundState = State.DISMISSED;
    }
    void OnDestroy()
    {
        if (Background == BackgroundType.RewardBackground)
        {
            EventBus.DisplayRewardBackground -= OnDisplayBackground;
        }
        else
        {
            EventBus.DisplayNormalBackground -= OnDisplayBackground;
        }
        EventBus.AutomaticallyDismissForeground -= OnAutomaticallyDismissed;
        EventBus.SetBackgroundTouchEnabled -= OnSetBackgroundTouchEnabled;
    }

    void OnDisplayBackground(bool touchDismissible)
    {
        if (BackgroundState != State.SHOWING)
        {
            BackgroundState = State.SHOWING;
            TouchDismissible = touchDismissible;
            gameObject.SetActive(true);
        }
    }

    void OnSetBackgroundTouchEnabled(bool touchDismissible)
    {
        TouchDismissible = touchDismissible;
    }

    /// <summary>
    /// Triggered by EventTrigger component in inspector
    /// </summary>
    public void OnDismissed()
    {
        if (TouchDismissible)
        {
            Dismiss();
        }
    }

    private void Dismiss()
    {
        if (BackgroundState != State.DISMISSED)
        {
            BackgroundState = State.DISMISSED;
            EventBus.DismissForeground?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void OnAutomaticallyDismissed()
    {
        Dismiss();
    }
}
