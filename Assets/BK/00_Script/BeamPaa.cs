using UnityEngine;


public class BeamPaa : MonoBehaviour
{
   [SerializeField]private int _damage;
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (TryGetComponent(out AbstractHealthSystem _playerHealth))
      {
         _playerHealth.OnDamaged.Invoke(_damage);
      }
   }
}
