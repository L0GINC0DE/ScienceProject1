using UnityEngine;

public class TanhwnBalsa : AbstractPatton
{
   [SerializeField]private GameObject _bulletPrefab;
   [SerializeField]private Transform _bulletSpawn;
   public override void Patton()
   {
      Instantiate(_bulletPrefab, _bulletSpawn.position, _bulletSpawn.rotation);
   }
   
}
