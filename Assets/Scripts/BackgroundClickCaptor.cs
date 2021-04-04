using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundClickCaptor : MonoBehaviour
{
    [SerializeField] private EventBus EventBus;
    [SerializeField] private AnimationDispatcher Anim;

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
        Debug.Log("start is called dispite active false");
        EventBus.DisplayBackground += OnDisplayBackground;
        EventBus.AutomaticallyDismissForeground += OnAutomaticallyDismissed;
        EventBus.SetBackgroundTouchEnabled += OnSetBackgroundTouchEnabled;
        gameObject.SetActive(false);
        BackgroundState = State.DISMISSED;
    }
    void OnDestroy()
    {
        EventBus.DisplayBackground -= OnDisplayBackground;
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
