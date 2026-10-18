using System;
using DG.Tweening;
using UnityEngine;

public class Pattern : MonoBehaviour
{
   [SerializeField] private BK_PlayerPronunciationState playerState;
   [SerializeField] private float _power = 1;

   private void Update()
   {
      if (playerState.nowState == PronunciationState.None)
      {
         playerState.gameObject.transform.position = transform.position;
      }

      if (playerState.nowState == PronunciationState.Plus)
      {
         playerState.transform.DOMove(playerState.transform.position, _power).SetEase(Ease.Linear);
      }
   }
}
