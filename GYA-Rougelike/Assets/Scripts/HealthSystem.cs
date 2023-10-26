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

    public void Heal(int HealAmount)
    {
        Health += HealAmount;

        // Make sure it doesnt heal more than its allowed too
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
            HealthBarScript.UpdateBar(Health - (int)HealthBarScript.Slider.value);
        }
        else
        {
            HealthBarScript.UpdateBar(HealAmount);
        }
    }

    public void AddDefence(int DefenceToAdd)
    {
        Defence += DefenceToAdd;
    }

    // Function that returns a bool for if dead
    public bool TakeDamage(int Damage)
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

            return true;
        }
        else
        {
            return false;
        }
    }

    public void Die()
    {
        // Removes self
        Destroy(gameObject);

        // Removes the attached HealthBar
        Destroy(HealthBarScript.gameObject);
    }
}
