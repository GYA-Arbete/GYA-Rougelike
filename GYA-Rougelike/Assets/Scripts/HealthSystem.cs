using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    public int MaxHealth;
    public int Health;
    public int Defence = 0;

    [Header("HealthBar Stuff")]
    public BarScript HealthBarScript;

    // Called when spawning Players
    public void SetHealth(BarScript Script)
    {
        HealthBarScript = Script;

        Health = MaxHealth;
        Defence = 0;
    }

    public void AddDefence(int DefenceToAdd)
    {
        Defence += DefenceToAdd;
    }

    public void TakeDamage(int Damage)
    {
        if (Defence > 0)
        {
            Defence -= Damage;
            if (Defence < 0)
            {
                Health -= Math.Abs(Defence);
                HealthBarScript.UpdateBar(-Math.Abs(Defence));

                Defence = 0;
            }
        }
        else
        {
            Health -= Damage;
            HealthBarScript.UpdateBar(-Damage);
        }

        if (Health  <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Removes self
        Destroy(gameObject);
    }
}
