using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetBoolAnimation(string animationName, bool torf)
    {
        animator.SetBool(animationName, torf);
    }
    
    public void SetTriggerAnimation(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    public void NotifyAnimation()
    {
        Debug.Log("End", this);
        TurnManager.Instance.CompleteAction();
    }
}
