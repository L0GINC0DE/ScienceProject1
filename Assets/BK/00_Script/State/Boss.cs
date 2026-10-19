using UnityEngine;
using System;

public class Boss:MonoBehaviour
{
   public StateMachine stateMachine;
   private void Awake()
   {
      stateMachine = new StateMachine();
      stateMachine.Awake();
   }

   private void Update()
   {
      stateMachine.Update();
   }
}
