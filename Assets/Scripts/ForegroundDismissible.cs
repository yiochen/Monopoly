using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for UI components that are shown in the foreground with dark
/// background overlay. For example, ChanceCard.
/// The UI component can be dismissed by tapping the background overlay.
/// This class provides methods to override before showing the component and
/// after the component is dismissed.
/// </summary>
public class ForegroundDismissible : MonoBehaviour
{
    protected enum State
    {
        INITIALIZING,
        DISMISSED,
        SHOWING,
    }

    private State ItemState = State.INITIALIZING;
    [SerializeField] protected EventBus EventBus;

    private bool InitiallyHidden = true;
    /// <summary>
    /// Called when the component is started
    /// </summary>
    /// <returns>true for initially hiding the gameObject</returns>
    protected virtual bool Initialize()
    {
        return true;
    }

    /// <summary>
    /// Called when the component is being destroyed
    /// </summary>
    protected virtual void Cleanup()
    {

    }

    /// <summary>
    /// Called before showing the card
    /// </summary>
    protected virtual void OnPrepare()
    {

    }

    protected void SetTouchDismissible(bool dismissible)
    {
        EventBus.SetBackgroundTouchEnabled?.Invoke(dismissible);
    }

    /// <summary>
    /// Called when the card is dimissed by tapping on the background
    /// </summary>
    protected virtual void OnDismiss()
    {

    }

    /// <summary>
    /// Show tthe item
    /// </summary>
    protected void Show()
    {
        if (ItemState != State.SHOWING)
        {
            ItemState = State.SHOWING;
            gameObject.SetActive(true); // Set active before OnPrepare because OnPrepare might play animation.
            OnPrepare();
            EventBus.DisplayRewardBackground?.Invoke(false);
            EventBus.DismissForeground += InternalOnDismissed;

        }
    }

    private void InternalOnDismissed()
    {
        if (ItemState != State.DISMISSED)
        {
            ItemState = State.DISMISSED;
            EventBus.DismissForeground -= InternalOnDismissed;
            if (InitiallyHidden)
            {
                gameObject.SetActive(false);
            }
            OnDismiss();
        }
    }
    void Start()
    {
        InitiallyHidden = Initialize();
        ItemState = InitiallyHidden ? State.DISMISSED : State.SHOWING;
        if (InitiallyHidden)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        Cleanup();
    }
}
