using UnityEngine;
using System;
using Mirror;

public class HealthSystem : NetworkBehaviour
{
    [Header("Variables")]
    [SyncVar]
    public int MaxHealth;
    [SyncVar]
    public int Health;
    [SyncVar]
    public int Defence = 0;
    [SyncVar]
    public bool Player = false;

    [Header("Player Exclusive Variables")]
    [SyncVar]
    public int Thorns = 0;

    [Header("HealthBar Stuff")]
    public BarScript HealthBarScript;

    // Called when spawning Players
    public void SetHealth(BarScript Script)
    {
        HealthBarScript = Script;

        Health = MaxHealth;
        Defence = 0;

        Player = true;
    }

    public void SetupEnemy(BarScript Script)
    {
        HealthBarScript = Script;

        Health = MaxHealth;
        Defence = 0;
    }

    public void ResetHealth()
    {
        Health = MaxHealth;
        Defence = 0;

        gameObject.SetActive(true);
        HealthBarScript.GetComponent<Transform>().gameObject.SetActive(true);

        HealthBarScript.ResetBar();
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

        HealthBarScript.UpdateDefence(Defence);
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

            HealthBarScript.UpdateDefence(Defence);
        }
        else
        {
            Health -= Damage;
            HealthBarScript.UpdateBar(-Damage);
        }

        // Check if dead
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
        if (Player)
        {
            gameObject.SetActive(false);
            HealthBarScript.GetComponent<Transform>().gameObject.SetActive(false);
        }
        // If an Enemy
        else
        {
            // Removes self
            Destroy(gameObject);

            // Removes the attached HealthBar
            Destroy(HealthBarScript.gameObject);
        }
    }
}
