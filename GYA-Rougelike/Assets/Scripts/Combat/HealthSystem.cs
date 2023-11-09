using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Variables")]
    public int MaxHealth;
    public int Health;
    public int Defence = 0;
    public bool Player = false;

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
        else
        {
            // Removes self
            Destroy(gameObject);

            // Removes the attached HealthBar
            Destroy(HealthBarScript.gameObject);
        }
    }
}
