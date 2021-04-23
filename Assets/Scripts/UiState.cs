using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiState<T> where T : MonoBehaviour
{
    public virtual void Init(T monoBehavior) { }

    public virtual void Render(T monoBehavior) { }

    public virtual UiState<T> TryTransition(T monoBehavior) { return null; }
}
