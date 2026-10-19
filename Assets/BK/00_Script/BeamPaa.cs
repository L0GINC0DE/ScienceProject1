using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeamPaa : MonoBehaviour
{
   
   private CancellationTokenSource _source;

   private void Awake()
   {
      _source = new CancellationTokenSource();
   }
   
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (TryGetComponent(out AbstractHealthSystem _playerHealth))
      {
         _playerHealth.OnDamaged.Invoke(1);
         Task().Forget();
      }
   }
   

   public async UniTaskVoid Task()
   {
      try
      {
         await UniTask.Delay(3000,cancellationToken: _source.Token);
         gameObject.SetActive(false);
      }
      catch (OperationCanceledException e)
      {
         Debug.Log("취소");
      }
   }
}
