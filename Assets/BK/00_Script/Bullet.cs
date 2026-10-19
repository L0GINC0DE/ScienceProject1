using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Update()
    {
        gameObject.transform.position += new Vector3(-0.1f, 0, 0);
    }
}
