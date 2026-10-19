using System;
using UnityEngine;

public class BossAnimationCom : MonoBehaviour
{
  private Animator _animator;

  private void Awake()
  {
    _animator = GetComponent<Animator>();
  }
}
