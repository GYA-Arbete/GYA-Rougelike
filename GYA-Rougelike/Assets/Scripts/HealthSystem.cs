using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Play : MonoBehaviour
{
    public int MaxHealth;
    public int Health;
    public int Defence = 0;

    // Called when restarting the map
    void Restart()
    {
        Health = MaxHealth;
        Defence = 0;
    }

    void TakeDamage(int Damage)
    {
        if (Defence > 0)
        {
            Defence -= Damage;
            if (Defence < 0)
            {
                Health -= Math.Abs(Defence);
                Defence = 0;
            }
        }
        else
        {
            Health -= Damage;
        }

        if (Health  < 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Do stuff
    }
}
