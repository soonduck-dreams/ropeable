using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IRopeable
{
    private int attack = 1;

    public string GetTerrainType()
    {
        return "obstacle";
    }

    public void OnRopeHang()
    {

    }

    public void OnRopeCut()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.OnDamage(attack);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if(damageable != null)
        {
            damageable.OnDamage(attack);
        }
    }
}
