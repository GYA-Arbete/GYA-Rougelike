using UnityEngine;

// Simple script for holding the values of the card its attached too
public class CardStats : MonoBehaviour
{
    [Header("Card Stats")]
    public int Energy;
    public int Damage;
    public bool SplashDamage;
    public int Defence;
    public int Cooldown;

    public void AssignValues(int energy, int damage, bool splash, int defence, int cooldown)
    {
        Energy = energy;
        Damage = damage;
        SplashDamage = splash;
        Defence = defence;
        Cooldown = cooldown;
    }
}
