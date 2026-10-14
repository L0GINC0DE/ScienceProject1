using System;
using UnityEngine;

public class BossHealthSystem : AbstractHealthSystem
{
    private void Start()
    {
        SetHealth(1000);
    }
}
