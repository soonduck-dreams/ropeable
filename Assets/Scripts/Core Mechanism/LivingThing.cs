using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingThing : MonoBehaviour, IDamageable
{
    public int currentHP { get; private set; }

    protected int initialHP { get; private set; }
    protected int maxHP { get; private set; }

    public virtual void OnDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public virtual void OnForceDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public virtual void RestoreHP(int restore)
    {
        currentHP += restore;

        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    protected abstract void Die();

    protected void SetupHP(int initial, int max)
    {
        initialHP = initial;
        maxHP = max;

        currentHP = initialHP;
    }
}