using System;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public Animator animator;

    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
