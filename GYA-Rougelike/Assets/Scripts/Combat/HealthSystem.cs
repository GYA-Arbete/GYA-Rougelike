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
    [SyncVar]
    public int PlayerIndex = -1;

    [Header("Player Exclusive Variables")]
    [SyncVar]
    public int Thorns = 0;

    [Header("HealthBar Stuff")]
    public BarScript HealthBarScript;

    [Command(requiresAuthority = false)]
    public void SetupObject(int MaxHp, bool PlayerBool, int Playerindex)
    {
        PlayerIndex = Playerindex;
        Player = PlayerBool;
        MaxHealth = MaxHp;
        Health = MaxHp;
        Defence = 0;
    }

    // Called from ClientRpc
    public void SetBarScript(BarScript Script)
    {
        HealthBarScript = Script;
    }

    [Command(requiresAuthority = false)]
    public void ResetPlayerHealth()
    {
        Health = MaxHealth;
        Defence = 0;

        ResetHealthBar();
    }

    [ClientRpc]
    void ResetHealthBar()
    {
        HealthBarScript.ResetBar();
    }

    [Command(requiresAuthority = false)]
    public void Heal(int HealAmount)
    {
        Health += HealAmount;

        // Make sure it doesnt heal more than its allowed too
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
            UpdateHealthBar(Health - (int)HealthBarScript.Slider.value);
        }
        else
        {
            UpdateHealthBar(HealAmount);
        }
    }

    [Command(requiresAuthority = false)]
    public void AddDefence(int DefenceToAdd)
    {
        Defence += DefenceToAdd;

        UpdateHealthBarDefence(Defence);
    }

    [Command(requiresAuthority = false)]
    public void AddThorns(int ThornsToAdd)
    {
        Thorns += ThornsToAdd;
    }

    // Function that returns a bool for if dead
    [Server]
    public bool TakeDamage(int Damage)
    {
        if (Defence > 0)
        {
            Defence -= Damage;
            if (Defence < 0)
            {
                Health -= Math.Abs(Defence);
                UpdateHealthBar(-Math.Abs(Defence));

                Defence = 0;
            }

            UpdateHealthBarDefence(Defence);
        }
        else
        {
            Health -= Damage;
            UpdateHealthBar(-Damage);
        }

        // Check if dead
        if (Health <= 0)
        {
            Die();

            return true;
        }
        else
        {
            return false;
        }
    }

    [ClientRpc]
    void UpdateHealthBar(int ChangeFactor)
    {
        HealthBarScript.UpdateBar(ChangeFactor);
    }

    [ClientRpc]
    void UpdateHealthBarDefence(int ChangeFactor)
    {
        HealthBarScript.UpdateDefence(ChangeFactor);
    }

    [Command(requiresAuthority = false)]
    public void Die()
    {
        if (Player)
        {
            FindAnyObjectByType<PlayerManager>().PlayerDie(PlayerIndex);
        }
        // If an Enemy
        else
        {
            NetworkServer.Destroy(HealthBarScript.gameObject);

            // Removes self
            NetworkServer.Destroy(gameObject);
        }
    }
}
