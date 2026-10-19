using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pattern : MonoBehaviour
{
   [SerializeField] private BK_PlayerPronunciationState playerState;
   [SerializeField] private float _power = 1;
  
   
   private void Update()
   {

       if (Keyboard.current.spaceKey.wasPressedThisFrame)
       {
           if (playerState.nowState == PronunciationState.Plus)
           {
               playerState.nowState = PronunciationState.Plus;
           }
       }
       
   }
}
