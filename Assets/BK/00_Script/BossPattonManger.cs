using System;
using System.Collections.Generic;
using UnityEngine;

public class BossPattonManger : MonoBehaviour
{
  public static BossPattonManger instance;
  public List<AbstractPatton> bosses = new List<AbstractPatton>();

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
