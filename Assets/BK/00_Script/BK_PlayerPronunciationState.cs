using System;
using UnityEngine;

public class BK_PlayerPronunciationState : MonoBehaviour
{
    public PronunciationState nowState;
    public Rigidbody2D Rb { get;private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        nowState = PronunciationState.Plus;
    }
}

public enum PronunciationState
{
    Plus,
    Minus,
    None
}